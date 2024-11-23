using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Extensions.Models
{
    public class ConfigureOIDCRoleRequest
    {
        public string[]? policies { get; set; }
        public string? bound_audiences { get; set; }
        public string[]? allowed_redirect_uris { get; set; }
        public string? user_claim { get; set; }
        public string? groups_claim { get; set; }
    }

}
