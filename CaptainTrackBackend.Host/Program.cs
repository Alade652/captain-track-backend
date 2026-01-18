using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Application.Services;
using CaptainTrackBackend.Application.Services.FileUpload;
using CaptainTrackBackend.Application.Services.ServiceProviders;
using CaptainTrackBackend.Host.Extension;
using CaptainTrackBackend.Persistence;
using CaptainTrackBackend.Persistence.RealTime;
using CloudinaryDotNet;
using Firebase.Database;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using dotenv.net;


var builder = WebApplication.CreateBuilder(args);

// Load .env file if it exists (using dotenv.net)
DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { ".env" }, ignoreExceptions: true));

// Configure configuration sources - Environment variables override appsettings.json
// Priority: .env file → appsettings.json → appsettings.{Environment}.json → Environment Variables → User Secrets
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(optional: true);

// Firebase initialization - Simple file-based approach
// Priority: 1) File path from config/env, 2) Default file name, 3) Environment variable JSON
GoogleCredential credential;

// Try file first (simplest approach)
var firebaseServiceAccountPath = builder.Configuration["Firebase:ServiceAccountKeyPath"] 
    ?? Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_KEY_PATH")
    ?? "serviceAccountKey.json";

// Check multiple possible locations
var possiblePaths = new[]
{
    firebaseServiceAccountPath,
    Path.Combine(Directory.GetCurrentDirectory(), firebaseServiceAccountPath),
    Path.Combine(AppContext.BaseDirectory, firebaseServiceAccountPath),
    "serviceAccountKey.json",
    Path.Combine(Directory.GetCurrentDirectory(), "serviceAccountKey.json")
};

string? foundPath = null;
foreach (var path in possiblePaths)
{
    if (File.Exists(path))
    {
        foundPath = path;
        break;
    }
}

if (!string.IsNullOrEmpty(foundPath))
{
    // Load from file (simplest and most reliable)
    credential = GoogleCredential.FromFile(foundPath);
    Console.WriteLine($"[Auth] Successfully loaded Firebase credentials from file: {foundPath}");
}
else
{
    // Fallback to environment variable if file not found
    var firebaseServiceAccountJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");
    if (!string.IsNullOrEmpty(firebaseServiceAccountJson))
    {
        credential = GoogleCredential.FromJson(firebaseServiceAccountJson);
        Console.WriteLine("[Auth] Successfully loaded Firebase credentials from environment variable.");
    }
    else
    {
        throw new FileNotFoundException(
            $"Firebase service account key not found. " +
            $"Checked paths: {string.Join(", ", possiblePaths)}. " +
            $"Please add 'serviceAccountKey.json' to the project root or set FIREBASE_SERVICE_ACCOUNT_JSON environment variable.");
    }
}

FirebaseApp.Create(new AppOptions
{
    Credential = credential
});

var firebaseDatabaseUrl = builder.Configuration["Firebase:DatabaseUrl"] 
    ?? Environment.GetEnvironmentVariable("FIREBASE_DATABASE_URL")
    ?? throw new InvalidOperationException(
        "Firebase Database URL is not configured. " +
        "Please set Firebase:DatabaseUrl in appsettings.json or FIREBASE_DATABASE_URL environment variable.");

builder.Services.AddSingleton(_ => new FirebaseClient(firebaseDatabaseUrl));



builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton<Cloudinary>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
    return new Cloudinary(account);
});

builder.Services.AddHttpClient<ISmsService, HollatagsSmsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            // Fallback for development - restrict in production!
            if (builder.Environment.IsDevelopment())
            {
                policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:4200")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
            else
            {
                throw new InvalidOperationException(
                    "CORS allowed origins are not configured. " +
                    "Please set Cors:AllowedOrigins in appsettings.json or environment variables.");
            }
        }
        
               //.AllowCredentials(); // If you�re using cookies or authorization
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

string connectionString = builder.Configuration.GetConnectionString("CaptainTrackConnectionString");
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(5, 5, 62))));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.RegisterJWT(builder.Configuration);

builder.Services.AddSignalR();


var app = builder.Build();

// Test database connection on startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        
        logger.LogInformation("═══════════════════════════════════════════════════════");
        logger.LogInformation("🔍 Testing database connection...");
        
        var canConnect = dbContext.Database.CanConnect();
        
        if (canConnect)
        {
            var connection = dbContext.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            
            var databaseName = connection.Database;
            var serverVersion = connection.ServerVersion;
            var serverName = connection.DataSource;
            
            logger.LogInformation("✅ DATABASE CONNECTION SUCCESSFUL!");
            logger.LogInformation("   📊 Database: {Database}", databaseName);
            logger.LogInformation("   🖥️  Server: {Server}", serverName);
            logger.LogInformation("   🔢 Version: {Version}", serverVersion);
            logger.LogInformation("═══════════════════════════════════════════════════════");
            
            connection.Close();
        }
        else
        {
            logger.LogError("═══════════════════════════════════════════════════════");
            logger.LogError("❌ DATABASE CONNECTION FAILED!");
            logger.LogError("   Cannot connect to database.");
            logger.LogError("   Please check your connection string in .env file");
            logger.LogError("═══════════════════════════════════════════════════════");
        }
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError("═══════════════════════════════════════════════════════");
    logger.LogError("❌ DATABASE CONNECTION ERROR!");
    logger.LogError("   Error: {Message}", ex.Message);
    logger.LogError("   Please verify your connection string in .env file or appsettings.json");
    logger.LogError("═══════════════════════════════════════════════════════");
}

app.MapHub<LocationHub>("/locationHub");
app.MapHub<NegotiationHub>("/negotiationHub");
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<BookingHub>("/bookingHub");



app.UseHttpsRedirection();

app.UseStaticFiles(); // To serve uploaded files if needed

app.UseCors("AllowSpecificOrigin");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// Only enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CaptainTrackApi");
    });
}


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.Run();

/*var sampleTodos = new Todo[] {
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());*/



/*public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}*/
