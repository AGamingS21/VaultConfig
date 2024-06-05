using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Extensions.Models
{
    public class ConfigureOIDCAuthRequest
    {
        public string oidc_discovery_url { get; set; }
        public string oidc_client_id { get; set; }
        public string oidc_client_secret { get; set; }
        public string default_role { get; set; }
    }

}
