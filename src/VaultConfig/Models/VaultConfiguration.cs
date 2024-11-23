using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace VaultConfig.Models
{
    public class VaultConfiguration
    {
        public List<PasswordConfig>? passwords { get; set; }
        public List<Auth>? auth { get; set; }
        public List<Policies>? policies { get; set; }
        public List<Group>? groups { get; set; }
        
        [YamlMember(typeof(List<GroupAliases>), Alias = "group-aliases")]
        public List<GroupAliases>? groupAliases { get; set; }
        public List<string>? envs {get; set;}

    }

    public class PasswordConfig
    {
        public string? path { get; set; }
        public List<Passwords>? data { get; set; }
    }

    public class Passwords
    {
        public string? key { get; set; }
        public int length { get; set; }
        public bool special { get; set; }
    }

    public class Roles
    {
        public string? name { get; set; }
        public string[]? policies { get; set; }
        public string? bound_audiences { get; set; }
        public string[]? allowed_redirect_uris { get; set; }
        public string? user_claim { get; set; }
        public string? groups_claim { get; set; }
    }


    public class Auth
    {
        public string? type { get; set; }
        public string? description { get; set; }
        public string? path { get; set; }
        public string? oidc_discovery_url { get; set; }
        public string? oidc_client_id { get; set; }
        public OIDCSecretValue? oidc_client_secret { get; set; }
        public string? default_role { get; set; }
        public List<Roles>? roles { get; set; }
        public List<UserPass>? users {get; set;}
        
    }

    public class OIDCSecretValue
    {
        public string? value {get; set;}
        public bool useFromKVEngine {get; set;}
    }
    public class UserPass
    {
        public string? username {get; set;}
        public string? password {get; set;}
        public bool useFromKVEngine {get; set;}
        public string[]? token_policies {get; set;}
    }

    public class Policies
    {
        public string? name { get; set; }
        public string? rules { get; set; }
    }

    public class Group
    {
        public string? name { get; set; }
        public string[]? policies { get; set; }
        public Dictionary<string, string>? metadata { get; set; }
        public string? type { get; set; }
    }

    public class GroupAliases
    {
        public string? name { get; set; }
        public string? mountPath { get; set; }
        public string? group { get; set; }
    }

}
