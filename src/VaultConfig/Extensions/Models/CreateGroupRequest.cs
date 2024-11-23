using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Extensions.Models
{


    public class CreateGroupRequest
    {
        public string? name { get; set; }
        public string[]? policies { get; set; }
        public Dictionary<string, string>? metadata { get; set; }
        public string? type { get; set; }
    }

}
