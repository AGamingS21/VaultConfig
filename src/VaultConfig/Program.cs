using Newtonsoft.Json;
using Keycloak;
using Keycloak.Net;
using YamlDotNet.Serialization;
using VaultConfig.Helpers;
using VaultConfig.Models;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Flurl.Util;
using System.Formats.Tar;
using System.Linq;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using Flurl.Http;
using VaultSharp.V1.SecretsEngines;
using VaultConfig.Extensions;
using Serilog;

namespace VaultConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            Log.Information("Starting Program");

            string rootTokenFile = Environment.GetEnvironmentVariable("TOKEN_FILE");
            string vaultAddress = Environment.GetEnvironmentVariable("VAULT_ADDRESS");
            string yamlFilePath = Environment.GetEnvironmentVariable("CONFIG_FILE");
            string environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
            string importFile = Environment.GetEnvironmentVariable("IMPORT_FILE");

            string uploadUriAndRootToken = Environment.GetEnvironmentVariable("UPLOAD_TOKEN");

            bool uploadRootToken = false;
            if(uploadUriAndRootToken != null && uploadUriAndRootToken.ToLower() == "true")
                uploadRootToken = true;

            if(yamlFilePath is null)
                yamlFilePath = "/config/config.yml";

            if(rootTokenFile is null)
                rootTokenFile = "/config/token.json";

            if(importFile is null)
                importFile = "/config/import.yml";

            if(environment is null)
                environment = "";

            if(environment != "Dev")
                Thread.Sleep(5000);

            VaultConfigurer vaultConfigurer = new VaultConfigurer(rootTokenFile, vaultAddress, yamlFilePath, uploadRootToken);
            vaultConfigurer.CreateAccess();
            vaultConfigurer.CreateSecrets();                        
            vaultConfigurer.CreateAuth();


            if(File.Exists(importFile))
            {
                Log.Information("Import File has been found at " + importFile);    
                
                vaultConfigurer.ImportSecrets(importFile);

                if(environment != "Dev")
                    File.Delete(importFile);
            }
            

            Log.Information("Waiting for 5 mins before ending");
            if(environment != "Dev")
                Thread.Sleep(300000);
            Log.Information("Ending Program");



            // Keycloak Configuration
            //var keycloakClient = new KeycloakClient("http://keycloak.local.thesafer.net", "admin", "admin", new KeycloakOptions(adminClientId: "admin-cli"));
            //var testing = keycloakClient.GetUserAsync("thesafer", "6aa54f86-18ea-4c56-80a7-aedff69805b3");

            //testing.Wait();



            //var entities = vaultClient.Identity.EntityListById();

            //foreach(var entity in entities.Data.Keys)
            //{
            //    var test1 = vaultClient.Identity.EntityReadById(entity);

            //    var item = JsonConvert.DeserializeObject<Rootobject>(test1.Data.ToString());



            //    Console.WriteLine(test1.Data.ToString());

            //}


        }

        
    }


    
}
