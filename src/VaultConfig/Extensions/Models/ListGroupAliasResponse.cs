using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Extensions.Models
{
    public class GroupAlias
    {
        public string? id { get; set; }
        public string? mount_accessor { get; set; }
        public string? mount_path { get; set; }
        public string? mount_type { get; set; }
        public string? name { get; set; }
    }

    public class KeyInfoGroupAlias
    {
        public string? canonical_id { get; set; }
        public string? custom_metadata { get; set; }
        public bool local { get; set; }
        public string? mount_accessor { get; set; }
        public string? mount_path { get; set; }
        public string? mount_type { get; set; }
        public string? name { get; set; }
    }

    public class GroupData
    {
        
        public Dictionary<string, KeyInfoGroupAlias>? key_info { get; set; }
        public List<string>? keys { get; set; }
        
    }

    public class ListGroupAliasResponse
    {
        public string? request_id { get; set; }
        public string? lease_id { get; set; }
        public bool renewable { get; set; }
        public int lease_duration { get; set; }
        public GroupData? data { get; set; }
        public object? wrap_info { get; set; }
        public object? warnings { get; set; }
        public object? auth { get; set; }
    }
}
