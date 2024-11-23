using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Extensions.Models
{


    public class Alias
    {
        public string? id { get; set; }
        public string? mount_accessor { get; set; }
        public string? mount_path { get; set; }
        public string? mount_type { get; set; }
        public string? name { get; set; }
    }

    public class KeyInfo
    {
        public Alias? alias { get; set; }
        public string? name { get; set; }
        public int num_member_entities { get; set; }
        public int num_parent_groups { get; set; }
    }

    public class Data
    {
        public Dictionary<string, KeyInfo>? key_info { get; set; }
        public List<string>? keys { get; set; }
    }

    public class ListGroupsResponse
    {
        public string? request_id { get; set; }
        public string? lease_id { get; set; }
        public bool renewable { get; set; }
        public int lease_duration { get; set; }
        public Data? data { get; set; }
        public object? wrap_info { get; set; }
        public object? warnings { get; set; }
        public object? auth { get; set; }
    }



}
