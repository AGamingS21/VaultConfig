using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Extensions.Models
{
    public class CreateGroupAliasRequest
    {
        public string? mount_accessor { get; set; }
        public string? name { get; set; }
        public string? canonical_id { get; set; }
    }
}
