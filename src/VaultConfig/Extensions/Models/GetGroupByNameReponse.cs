using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultConfig.Extensions.Models
{
    public class GetGroupAlias
    {
        public string canonical_id { get; set; }
        public string creation_time { get; set; }
        public string id { get; set; }
        public string last_update_time { get; set; }
        public string merged_from_canonical_ids { get; set; }
        public string metadata { get; set; }
        public string mount_accessor { get; set; }
        public string name { get; set; }
    }

    public class GetGroupData
    {
        public GetGroupAlias alias { get; set; }
        public string creation_time { get; set; }
        public string id { get; set; }
        public string last_update_time { get; set; }
        public string[] member_entity_ids { get; set; }
        public string[] member_group_ids { get; set; }
        public Dictionary<string, string> metadata { get; set; }
        public int modify_index { get; set; }
        public string name { get; set; }
        public string namespace_id { get; set; }
        public string[] parent_group_ids { get; set; }
        public string[] policies { get; set; }
        public string type { get; set; }
    }
    public class GetGroupByNameResponse
    {
        public string request_id { get; set; }
        public string lease_id { get; set; }
        public bool renewable { get; set; }
        public int lease_duration { get; set; }
        public GetGroupData data { get; set; }
        public object wrap_info { get; set; }
        public object warnings { get; set; }
        public object auth { get; set; }
    }
}
