using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace VaultConfig.Models
{
    public class ImportList
    {
        public List<ImportPasswords> passwords { get; set; }
    }

    public class ImportPasswords
    {
        public string path { get; set; }
        public string value { get; set; }
        public string name { get; set; }
        public bool import { get; set; }
    }
}
