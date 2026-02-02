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
using Microsoft.AspNetCore.DataProtection;
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

// Firebase initialization - Optional (commented out to disable)
// Uncomment the code below to enable Firebase
// Note: For production, use FIREBASE_SERVICE_ACCOUNT_JSON environment variable, never commit credentials to git
/*
GoogleCredential? credential = null;

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
        Console.WriteLine("[WARNING] Firebase credentials not found. Firebase features will be disabled.");
        // Don't throw - make it optional
    }
}

if (credential != null)
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = credential
    });

    var firebaseDatabaseUrl = builder.Configuration["Firebase:DatabaseUrl"] 
        ?? Environment.GetEnvironmentVariable("FIREBASE_DATABASE_URL");
    
    if (!string.IsNullOrEmpty(firebaseDatabaseUrl))
    {
        builder.Services.AddSingleton(_ => new FirebaseClient(firebaseDatabaseUrl));
        Console.WriteLine("[Auth] Firebase initialized successfully.");
    }
    else
    {
        Console.WriteLine("[WARNING] Firebase Database URL not configured. Firebase client not initialized.");
    }
}
*/



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

// Configure DataProtection to use environment-specific storage
// On Render, use a persistent directory or disable if not needed
if (builder.Environment.IsProduction())
{
    // Use a persistent directory for DataProtection keys in production
    var dataProtectionKeysPath = Environment.GetEnvironmentVariable("DATA_PROTECTION_KEYS_PATH") 
        ?? Path.Combine(Path.GetTempPath(), "DataProtection-Keys");
    Directory.CreateDirectory(dataProtectionKeysPath);
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new System.IO.DirectoryInfo(dataProtectionKeysPath));
}

// Use a temporary logger for early diagnostics
using var loggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
var startupLogger = loggerFactory.CreateLogger("Program");

string connectionString = builder.Configuration.GetConnectionString("CaptainTrackConnectionString") 
    ?? builder.Configuration["ConnectionStrings:CaptainTrackConnectionString"]
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__CaptainTrackConnectionString");

if (!string.IsNullOrEmpty(connectionString))
{
    var parts = connectionString.Split(';');
    var maskedParts = parts.Select(p => 
    {
        var trimmed = p.Trim();
        if (trimmed.StartsWith("password=", StringComparison.OrdinalIgnoreCase)) return "password=******";
        return p;
    });
    startupLogger.LogInformation("[DBDebug] ConnectionString found (Masked): {ConnectionString}", string.Join(";", maskedParts));
}
else
{
    startupLogger.LogError("[DBDebug] ERROR: CaptainTrackConnectionString is NULL or EMPTY. Checked GetConnectionString, Configuration direct, and Environment Variables.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options => 
{
    if (string.IsNullOrEmpty(connectionString))
    {
        options.UseMySql("server=localhost", new MySqlServerVersion(new Version(8, 0, 0)));
        return;
    }
    
    try 
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
    catch (Exception ex)
    {
        startupLogger.LogWarning("[DBDebug] Auto-detection failed: {Message}. Falling back to MySQL 8.0.", ex.Message);
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));
    }
});

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
        logger.LogInformation("🔍 Testing database connection and applying migrations...");
        
        // Apply pending migrations automatically
        try 
        {
            dbContext.Database.Migrate();
            logger.LogInformation("✅ Database migrations applied successfully!");
        }
        catch (Exception ex)
        {
            logger.LogWarning("⚠️ Could not apply migrations: {Message}. The database may already be up to date or lacks permissions.", ex.Message);
        }

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



// HTTPS redirection is handled by Render's load balancer
// Only enable HTTPS redirection in development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles(); // To serve uploaded files if needed

app.UseCors("AllowSpecificOrigin");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// Swagger: enabled in Development; in Production enable via ENABLE_SWAGGER=true if desired
var enableSwagger = app.Environment.IsDevelopment()
    || string.Equals(Environment.GetEnvironmentVariable("ENABLE_SWAGGER"), "true", StringComparison.OrdinalIgnoreCase);
if (enableSwagger)
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
