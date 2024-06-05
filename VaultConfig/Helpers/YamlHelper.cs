using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace VaultConfig.Helpers
{
    public static class YamlHelper
    {
        public static T AsObject<T>(string yamlFileString)
        {
            var yamlConfig = new StringReader(yamlFileString);
            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<T>(yamlConfig);
        }


    }
}