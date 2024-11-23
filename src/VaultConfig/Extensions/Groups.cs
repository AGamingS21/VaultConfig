using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines.Identity;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using VaultConfig.Extensions.Models;

namespace VaultConfig.Extensions
{

    public static class GroupsExtension
    {
        
        public static async Task<ListGroupsResponse> ListGroupsByIdAsync(string vaultUri, string token)
        {
            HttpClient httpClient = new HttpClient();
            string groupUri = vaultUri + "/v1/identity/group/id?list=true";

            httpClient.DefaultRequestHeaders.Add("X-Vault-Token", token);

            var response = await httpClient.GetAsync(groupUri);

            
       
            return await response.Content.ReadFromJsonAsync<ListGroupsResponse>();

        }

        public static async Task<CreateGroupResponse> CreateGroupAsync(string vaultUri, string token, CreateGroupRequest group)
        {
            HttpClient httpClient = new HttpClient();
            string groupUri = vaultUri + "/v1/identity/group";

            httpClient.DefaultRequestHeaders.Add("X-Vault-Token", token);

            var response = await httpClient.PostAsync(groupUri, new StringContent(JsonConvert.SerializeObject(group)));

            var body = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(body);


            return JsonConvert.DeserializeObject<CreateGroupResponse>(json["data"].ToString());
        }

        public static async Task<CreateGroupAliasResponse> CreateGroupAliasAsync(string vaultUri, string token, CreateGroupAliasRequest alias)
        {
            HttpClient httpClient = new HttpClient();
            string groupUri = vaultUri + "/v1/identity/group-alias";

            httpClient.DefaultRequestHeaders.Add("X-Vault-Token", token);

            var response = await httpClient.PostAsync(groupUri, new StringContent(JsonConvert.SerializeObject(alias)));

            var body = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(body);


            return JsonConvert.DeserializeObject<CreateGroupAliasResponse>(json["data"].ToString());
        }

        public static async Task<ListGroupAliasResponse> ListGroupAliasByIdAsync(string vaultUri, string token)
        {
            HttpClient httpClient = new HttpClient();
            string aliasUri = vaultUri + "/v1/identity/group-alias/id?list=true";

            httpClient.DefaultRequestHeaders.Add("X-Vault-Token", token);

            var response = await httpClient.GetAsync(aliasUri);



            return await response.Content.ReadFromJsonAsync<ListGroupAliasResponse>();
        }

        public static async Task<GetGroupByNameResponse> GetGroupByNameAsync(string vaultUri, string token, string groupName)
        {
            HttpClient httpClient = new HttpClient();
            string groupUri = vaultUri + "/v1/identity/group/name/" + groupName;

            httpClient.DefaultRequestHeaders.Add("X-Vault-Token", token);

            var response = await httpClient.GetAsync(groupUri);



            return await response.Content.ReadFromJsonAsync<GetGroupByNameResponse>();
        }



    }
}
