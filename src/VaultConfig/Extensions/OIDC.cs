using VaultConfig.Extensions.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Extensions
{
    public static class OIDCExtension
    {
        public static async Task ConfigureOIDCAuth(string vaultUri, string token, ConfigureOIDCAuthRequest configureOIDCAuth)
        {
            HttpClient httpClient = new HttpClient();
            string groupUri = vaultUri + "/v1/auth/oidc/config";

            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(configureOIDCAuth));

            httpClient.DefaultRequestHeaders.Add("X-Vault-Token", token);

            var response = await httpClient.PostAsync(groupUri, stringContent);



            return;

        }

        public static async Task CreateOIDCRole(string vaultUri, string token, ConfigureOIDCRoleRequest configureOIDCRoleRequest, string roleName)
        {
            HttpClient httpClient = new HttpClient();
            string groupUri = vaultUri + "/v1/auth/oidc/role/" + roleName;

            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(configureOIDCRoleRequest));

            httpClient.DefaultRequestHeaders.Add("X-Vault-Token", token);

            var response = await httpClient.PostAsync(groupUri, stringContent);



            return;

        }


    }
}
