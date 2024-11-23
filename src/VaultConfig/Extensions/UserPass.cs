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
    public static class UserPassExtension
    {
        public static async Task CreateUserAsync(string vaultUri, string token, CreateUserPassRequest createUserPassRequest, string username)
        {
            HttpClient httpClient = new HttpClient();
            string uri = vaultUri + $"/v1/auth/userpass/users/{username}";

            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(createUserPassRequest));

            httpClient.DefaultRequestHeaders.Add("X-Vault-Token", token);

            var response = await httpClient.PostAsync(uri, stringContent);



            return;

        }



    }
}
