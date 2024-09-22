using VaultConfig.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using Flurl.Util;
using System.IO;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.SecretsEngines;
using YamlDotNet.Core.Tokens;
using VaultSharp.V1.SystemBackend;
using VaultConfig.Extensions;
using VaultSharp.Core;
using VaultConfig.Extensions.Models;
using Serilog;
using VaultSharp.V1.Commons;
using VaultConfig.Helpers;
using Keycloak.Net.Models.Root;

namespace VaultConfig
{
    class VaultConfigurer
    {
        private VaultConfiguration? vaultConfig { get; set; }

        private VaultClient vaultClient { get; set; }

        private string vaultAddress { get; set; }
        
        private RootTokenFile rootTokenFile { get; set; }

        public VaultConfigurer(string rootTokenFilePath, string vaultAddress, string yamlFilePath, bool uploadRootToken)
        {
            this.vaultAddress = vaultAddress;
            //this.rootToken = rootToken;
            if (!CheckIfVaultInitialized())
            {
                VaultInit(rootTokenFilePath);
            }
            GetRootTokenFile(rootTokenFilePath);
            bool unsealed = UnsealVault();
            if (unsealed)
            {
                IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken: rootTokenFile.rootToken);
                var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod);
                this.vaultClient = new VaultClient(vaultClientSettings);

                this.vaultConfig = GetYamlConfig(yamlFilePath);
                Log.Information("Yaml configuration was successfully read.");
            }
            else
                Console.WriteLine("Issue with unsealing.");

            // If true then the root token password and the vault uri will be added as secrets to vault
            if(uploadRootToken)
            {
                List<Passwords> rootPasswords =
                [
                    new Passwords()
                    {
                        key = "vaultUri"
                    }
,
                    new Passwords()
                    {
                        key = "rootToken"
                    }
,
                ];

                vaultConfig.passwords.Add
                (
                    new PasswordConfig()
                    {
                        path = "admin/vault",
                        data = rootPasswords
                    }
                );

            }

        }

        #region Init

        


        private bool GetRootTokenFile(string rootTokenFilePath)
        {
            try { 
                var fileString = File.ReadAllText(rootTokenFilePath);
                rootTokenFile = JsonConvert.DeserializeObject<RootTokenFile>(fileString);
                Log.Information("Root Token Fille has been found");
                return true;
            }
            catch(Exception ex)
            {
                Log.Error("Root Token File is NOT Found\n" + ex.Message);
                return false;
            }

            
        }

        private bool UnsealVault()
        {
            try
            {
                var unseal = vaultClient.V1.System.UnsealAsync(rootTokenFile.unsealKey);
                unseal.Wait();
                Log.Information("Vault Unsealed");
                return true;

            }
            catch (Exception ex)
            {
                Log.Error("Count not unseal vault\n" + ex.Message);
                return false;
            }
        }

        private bool CheckIfVaultInitialized()
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken: "doesntmatter");
            var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod);
            vaultClient = new VaultClient(vaultClientSettings);
            var vaultInitStatus = vaultClient.V1.System.GetInitStatusAsync();
            vaultInitStatus.Wait();
            Log.Information("Vault initialized Status: " + vaultInitStatus.Result.ToString());
            return vaultInitStatus.Result;
        }
        private void VaultInit(string rootTokenFilePath)
        {   
            InitOptions initOptions = new InitOptions()
            {
                SecretThreshold = 1,
                SecretShares = 1
            };
            var init = vaultClient.V1.System.InitAsync(initOptions);
            init.Wait();
            Log.Information("Vault initialized");
            var initRootObject = new RootTokenFile()
            {
                rootToken = init.Result.RootToken,
                unsealKey = init.Result.Base64MasterKeys[0]
            };
            DeleteFileIfExists(rootTokenFilePath);
            using (StreamWriter writer = new StreamWriter(rootTokenFilePath, true, Encoding.ASCII))
            {
                writer.Write(JsonConvert.SerializeObject(initRootObject));
            }
            Log.Information("Vault Root token file created");
        }

        private void DeleteFileIfExists(string path)
        {
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }

        // Reads Config file from Yaml into an object
        private VaultConfiguration GetYamlConfig(string yamlFilePath)
        {
            var fileString = File.ReadAllText(yamlFilePath);
            var hiddenSecretsConfig = YamlHelper.AsObject<VaultConfiguration>(fileString);
            var removedSecrets = ReplaceEnvVariables(hiddenSecretsConfig.envs, fileString);
            // convert string/file to YAML object
            return YamlHelper.AsObject<VaultConfiguration>(removedSecrets);

        }

        

        private string ReplaceEnvVariables(List<string> placholders, string yamlContent)
            {
                // Find and replace environment variable placeholders in the YAML content
                // Assuming the placeholders are in the format ${ENV_VAR}
                string modifiedYamlContent = yamlContent;
                foreach (var placeholder in placholders)
                {
                    if (modifiedYamlContent.Contains(placeholder))
                    {
                        modifiedYamlContent = modifiedYamlContent.Replace(placeholder, Environment.GetEnvironmentVariable(placeholder));
                    }
                }
                return modifiedYamlContent;
            }


        #endregion

        #region Create Secretes
        // This will be used to check if the secrets engine exists. If not then create it.
        //TODO Add description?
        private void CheckSecretsEngine(VaultClient vaultClient, string engineName)
        {
            var engine = vaultClient.V1.System.GetSecretBackendsAsync();
            engine.Wait();
            var engineExists = engine.Result.Data.ToKeyValuePairs().Any(a => a.Key.Contains(engineName + "/"));


            // If the secrets engine does not exist create it.
            if (!engineExists)
            {
                //create secret engine version
                var dictionary = new Dictionary<string, object>
                {
                    { "version", "1" }
                };

                // Configure attributes of engine
                SecretsEngine secretsEngine = new SecretsEngine()
                {
                    Options = dictionary,
                    Path = engineName,
                    Type = new SecretsEngineType("kv")
                };


                var createSecretEngine = vaultClient.V1.System.MountSecretBackendAsync(secretsEngine);
                createSecretEngine.Wait();
            }
            Log.Information("Created Secrets engine: " + engineName);

        }

        private void WriteSecret(string engineName, string secretPath, Dictionary<string, string?> secretData)
        {

            var createKey = vaultClient.V1.Secrets.KeyValue.V1.WriteSecretAsync(secretPath, secretData, engineName);
            createKey.Wait();
        }

        // Check if a specific key already exists. if it does then return the keys in a dictionary.
        private Dictionary<string, string?> CheckIfKeyExists(string engineName, string secretPath, List<Passwords> passwords)
        {
            // Return all keys in secret. If none exist error will be thrown so just return empty list.
            Dictionary<string, string?> returnKeys;
            try
            {
                var readKeys = vaultClient.V1.Secrets.KeyValue.V1.ReadSecretAsync(secretPath, engineName);
                readKeys.Wait();
                var existingKeys = readKeys.Result.Data.ToKeyValuePairs();
                returnKeys = existingKeys.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
            }
            catch
            {
                returnKeys = new Dictionary<string, string?>();
            }

            Log.Information("Created Secret: " + secretPath);

            return returnKeys;
        }

        //Check to see what happens if you write dictionaries with secrets without ones that exist.
        public void CreateSecrets()
        {
            Dictionary<string, string?> passwords;

            foreach (var password in vaultConfig.passwords)
            {

                // split the first value from the rest of the path
                int index = password.path.IndexOf('/');
                string engineName = password.path.Substring(0, index);
                string secretPath = password.path.Substring(index + 1);

                // Ensure path is created
                //CreatePath(engineName, secretPath);

                // Create secrets engine if it does not already exist
                CheckSecretsEngine(vaultClient, engineName);

                //Find all of the passwords that exist.
                passwords = CheckIfKeyExists(engineName, secretPath, password.data);

                // Determine which passwords need to be created
                var passwordsToCreate = password.data.Where(kvp => !passwords.Any(item => item.Key == kvp.key)).ToList();
                foreach (var value in passwordsToCreate)
                {
                    
                    string newPassword;
                    
                    if(value.key == "rootToken")
                        newPassword = rootTokenFile.rootToken;
                    else if(value.key == "vaultUri")
                        newPassword = vaultAddress;
                    else
                        newPassword = Helpers.PasswordGenerator.GeneratePassword(value.length, 4, true, true, value.special, true);
                    
                    passwords.Add(value.key, newPassword);
                }

                // If there are passwords that need to be created write secret.
                if (passwordsToCreate.Count > 0)
                {
                    WriteSecret(engineName, secretPath, passwords);
                    Log.Information("Created Secret: " + secretPath);
                }
                
                


            }
        }
        #endregion

        #region Create Access

        public void CreateAccess()
        {
            foreach (var policy in vaultConfig.policies)
            {
                Log.Information("Checking policies that need to be created");
                ConfigurePolicy(policy);
            }
            foreach (var group in vaultConfig.groups)
            {
                Log.Information("Checking groups that need to be created");
                ConfigureGroups(group);
            }
            foreach (var groupAlias in vaultConfig.groupAliases)
            {
                Log.Information("Checking group alias that need to be created");
                ConfigureGroupAlias(groupAlias);
            }

        }


        // Configures Policies based on parameters passed in.
        public void ConfigurePolicy(Policies policy)
        {

            var policyList = vaultClient.V1.System.GetPoliciesAsync();
            policyList.Wait();

            bool policyExists;

            if (policyList.Result.Data != null)
                policyExists = policyList.Result.Data.ToKeyValuePairs().Any(a => a.Key.Contains(policy.name));
            else
                policyExists = false;

            if (!policyExists)
            {

                //Write Policy
                var newPolicy = new VaultSharp.V1.SystemBackend.Policy();
                newPolicy.Name = policy.name;
                newPolicy.Rules = policy.rules;
                var writePolicy = vaultClient.V1.System.WritePolicyAsync(newPolicy);
                writePolicy.Wait();
                Log.Information("Created policy: " + policy.name);
            }
            
        }

        // Creates / updates groups based
        // TODO: Do a comparison if the group does not match to the yaml and then delete/update if possible.
        public void ConfigureGroups(Group group)
        {

            //Get Groups 
            var groups = GroupsExtension.ListGroupsByIdAsync(vaultAddress, rootTokenFile.rootToken);
            groups.Wait();

            bool groupExists;

            if (groups.Result.data != null)
                groupExists = groups.Result.data.key_info.Any(a => a.Value.name.Contains(group.name));
            else
                groupExists = false;

            if (!groupExists)
            {
                CreateGroupRequest newGroup = new CreateGroupRequest()
                {
                    name = group.name,
                    metadata = group.metadata,
                    policies = group.policies,
                    type = group.type
                };

                var createGroup = GroupsExtension.CreateGroupAsync(vaultAddress, rootTokenFile.rootToken, newGroup);
                createGroup.Wait();
                Log.Information("Created group: " + group.name);
            }
        }

        // Creates / updates group alias
        // TODO: Do a comparison if the group alias does not match to the yaml and then delete/update if possible.
        private void ConfigureGroupAlias(GroupAliases groupAlias)
        {

            var currentAlias = GroupsExtension.ListGroupAliasByIdAsync(vaultAddress, rootTokenFile.rootToken);
            currentAlias.Wait();

            bool aliasExists;

            if (currentAlias.Result.data != null)
                aliasExists = currentAlias.Result.data.key_info.Any(a => a.Value.name.Contains(groupAlias.name));
            else
                aliasExists = false;

            if (!aliasExists)
            {
                var group = GroupsExtension.GetGroupByNameAsync(vaultAddress, rootTokenFile.rootToken, groupAlias.group);
                group.Wait();

                var authAccessor = vaultClient.V1.System.GetAuthBackendsAsync();
                authAccessor.Wait();

                var accessor = authAccessor.Result.Data[groupAlias.mountPath + "/"].Accessor;

                // Create Group Alias works but need error handling if it does not work.
                CreateGroupAliasRequest alias = new CreateGroupAliasRequest()
                {
                    name = groupAlias.name,
                    mount_accessor = accessor,
                    canonical_id = group.Result.data.id
                };
                var createGroupAlias = GroupsExtension.CreateGroupAliasAsync(vaultAddress, rootTokenFile.rootToken, alias);
                createGroupAlias.Wait();
                Log.Information("Created group alias: " + groupAlias.name);
            }


        }
        #endregion

        #region Create OIDC
        public void CreateAuth()
        {
            foreach (var authType in vaultConfig.auth)
            {
                switch (authType.type.ToLower())
                {
                    case "oidc":
                        ConfigureOIDC(authType);
                        break;
                }
            }
        }
        // Creates / udpates OIDC configuration based on parameters
        public void ConfigureOIDC(Auth authType)
        {
            // Get OIDC
            var auth = vaultClient.V1.System.GetAuthBackendsAsync();
            auth.Wait();

            bool authExists;

            if (auth.Result.Data != null)
                authExists = auth.Result.Data.ToKeyValuePairs().Any(a => a.Key.Contains(authType.path + "/"));
            else
                authExists = false;
            // If the auth identity engine with this path does not exist then create it.
            if (!authExists)
            {
                //Create OIDC
                AuthMethodType authMethodType = new AuthMethodType(authType.path);

                AuthMethod authMethod = new AuthMethod()
                {
                    Path = authType.path,
                    Description = authType.description,
                    Type = authMethodType
                };

                var createAuth = vaultClient.V1.System.MountAuthBackendAsync(authMethod);
                createAuth.Wait();
                Log.Information("Created oidc path: " + authType.path);
            }



            // Create/Update oidc auth method with path.
            ConfigureOIDCAuthRequest configureOIDCAuth = new ConfigureOIDCAuthRequest()
            {
                oidc_discovery_url = authType.oidc_discovery_url,
                oidc_client_id = authType.oidc_client_id,
                oidc_client_secret = authType.oidc_client_secret,
                default_role = authType.default_role
            };
            var configureOIDC = OIDCExtension.ConfigureOIDCAuth(vaultAddress, rootTokenFile.rootToken, configureOIDCAuth);
            configureOIDC.Wait();

            // For each role in the config create/update it.
            foreach (var role in authType.roles)
            {
                // Configure OIDC Role
                ConfigureOIDCRoleRequest OIDCRole = new ConfigureOIDCRoleRequest()
                {
                    policies = role.policies,
                    bound_audiences = role.bound_audiences,
                    allowed_redirect_uris = role.allowed_redirect_uris,
                    user_claim = role.user_claim,
                    groups_claim = role.groups_claim
                };
                var configureOIDCRole = OIDCExtension.CreateOIDCRole(vaultAddress, rootTokenFile.rootToken, OIDCRole, role.name);
                configureOIDCRole.Wait();
                Log.Information("Created oidc role: " + role.name);
            }

        }
        #endregion

        #region Import Secrets

        public void ImportSecrets(string importFile)
        {
            var fileString = File.ReadAllText(importFile);
            var importConfig = YamlHelper.AsObject<ImportList>(fileString);
            Dictionary<string, string?> passwords;

            foreach(var password in importConfig.passwords)
            {
                // split the first value from the rest of the path
                int index = password.path.IndexOf('/');
                string engineName = password.path.Substring(0, index);
                string secretPath = password.path.Substring(index + 1);
                // Create secrets engine if it does not already exist
                CheckSecretsEngine(vaultClient, engineName);

                // Create a password list so that this can be checked
                var passwordList = new List<Passwords>()
                {
                    new Passwords(){
                        key = password.value
                    }
                    
                };

                //Find all of the passwords that exist.
                passwords = CheckIfKeyExists(engineName, secretPath, passwordList);
                var exists = passwords.Any(item => item.Key == password.name);

                if(!exists)
                {
                    passwords.Add(password.name, password.value);
                    WriteSecret(engineName, secretPath, passwords);
                    Log.Information("Created Secret: " + secretPath);
                }    
            }
            
        }
        #endregion

    }
}
