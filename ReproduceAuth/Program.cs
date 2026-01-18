using System;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json.Linq;

namespace ReproduceAuth
{
    class Program
    {
        // A minimal valid dummy PEM key (generated for testing)
        static string ValidPemKey = 
@"-----BEGIN PRIVATE KEY-----
MIIBVQIBADANBgkqhkiG9w0BAQEFAASCAT8wggE7AgEAAkEAq+I8jJOxG21dvt/p
K8wTqE7NqNqOq5qJqOq5qJqOq5qJqOq5qJqOq5qJqOq5qJqOq5qJqOq5qJqOq5qJ
qOq5qwIDAQABAkEAnG+XjJqOq5qJqOq5qJqOq5qJqOq5qJqOq5qJqOq5qJqOq5qJ
qOq5qJqOq5qJqOq5qJqOq5qJqOq5awIhAP//////////AiEA//////////8CIQD/
/////////wIhAP//////////AiEA//////////8=
-----END PRIVATE KEY-----";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Reproduction with Required Fields...");

            // Template with minimal required fields for ServiceAccountCredential
            // Needs client_email and private_key at minimum for some checks, maybe token_uri
            string jsonTemplate = $@"{{
                ""type"": ""service_account"",
                ""project_id"": ""test-project"",
                ""private_key_id"": ""12345"",
                ""client_email"": ""test@test-project.iam.gserviceaccount.com"",
                ""client_id"": ""12345"",
                ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
                ""token_uri"": ""https://oauth2.googleapis.com/token"",
                ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
                ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/test%40test-project.iam.gserviceaccount.com"",
                ""private_key"": ""[KEY]""
            }}";

            // Test Case 1: Baseline (Valid JSON, keys correct)
            string validJson = jsonTemplate.Replace("[KEY]", ValidPemKey.Replace("\r\n", "\n").Replace("\n", "\\n"));
            RunTest("Baseline (Valid)", validJson);

            // Test Case 2: Double Escaped \n (e.g. literals in env var)
            // Simulates input: ... "private_key": "...\\n..." ...
            string doubleEscapedJson = jsonTemplate.Replace("[KEY]", ValidPemKey.Replace("\r\n", "\n").Replace("\n", "\\\\n"));
            RunTest("Double Escaped (literal \\n)", doubleEscapedJson, applyFix: true);

            // Test Case 3: Mixed Escaped \r\n (literals \\r\\n)
            string mixedEscapedJson = jsonTemplate.Replace("[KEY]", ValidPemKey.Replace("\r\n", "\n").Replace("\n", "\\\\r\\\\n"));
            RunTest("Mixed Escaped (literal \\r\\n)", mixedEscapedJson, applyFix: true);
             // Current FIX in Program.cs only replaces \\n -> \n, leaving \\r -> \r literal.
             // Result: ...\r[Newline]...
             // DecodeRsaParameters often handles whitespace but literal backslash?

            // Test Case 4: Spaces
            string spacesJson = jsonTemplate.Replace("[KEY]", ValidPemKey.Replace("\r\n", " ").Replace("\n", " "));
            RunTest("Spaces", spacesJson, applyFix: true);
        }

        static void RunTest(string name, string jsonInput, bool applyFix = false)
        {
            Console.WriteLine($"--- Test: {name} ---");
            try
            {
                GoogleCredential credential;
                try
                {
                    credential = GoogleCredential.FromJson(jsonInput);
                    Console.WriteLine("Success (Direct FromJson)");
                    // Force it to parse the key by creating a task or accessing it?
                    // Actually FromJson is just a factory. 
                    // To validate the key, we need to create a ServiceAccountCredential from it logic or rely on the factory doing it.
                    // The factory usually does minimal parsing.
                    // BUT the user stack trace says: GoogleCredential.FromJson -> DefaultCredentialProvider.CreateDefaultCredentialFromJson -> ... -> FromPrivateKey -> DecodeRsaParameters
                    // So FromJson DOES perform the parsing immediately.
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Direct FromJson failed: {ex.GetType().Name}: {ex.Message}");
                    if (!applyFix) return;
                }

                Console.WriteLine("Attempting Fix (Program.cs logic)...");
                var jobject = JObject.Parse(jsonInput);
                var privateKey = jobject["private_key"]?.ToString();
                
                if (!string.IsNullOrEmpty(privateKey))
                {
                    // EXACT Logic from Program.cs
                    var fixedKey = privateKey.Contains("\\n") 
                        ? privateKey.Replace("\\n", "\n") 
                        : privateKey;
                        
                    jobject["private_key"] = fixedKey;
                    
                    try {
                        credential = GoogleCredential.FromJson(jobject.ToString());
                        Console.WriteLine("Success (After Fix)");
                    } catch (Exception ex) {
                        Console.WriteLine($"FAILED After Fix: {ex.GetType().Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Unexpected Error: {ex.ToString()}");
            }
            Console.WriteLine();
        }
    }
}
