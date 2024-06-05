using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Extensions.Models
{
    public class RootTokenFile
    {
        public string rootToken { get; set; }
        public string unsealKey { get; set; }
    }
}
