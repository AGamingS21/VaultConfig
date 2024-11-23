using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Extensions.Models
{
    public class CreateUserPassRequest
    {
        public string? password { get; set; }
        public string[]? token_policies { get; set; }
    }

}
