using System;
using System.Collections.Generic;
using Strong531;
using YamlDotNet.Serialization;


namespace Strong531ConsoleApp
{
    class Configuration
    {
        public Configuration(string filename)
        {
            var yamlInput = System.IO.File.ReadAllText(filename);
            var deserializer = new DeserializerBuilder().Build();

            var config = deserializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(yamlInput);
            foreach (KeyValuePair<string, Dictionary<string, decimal>> user in config)
            {
                if (!users.ContainsKey(user.Key))
                {
                    users.Add(user.Key, new RepMax());
                }

                var repMax = users[user.Key];
                foreach (KeyValuePair<string, decimal> kv in user.Value)
                {
                    var lift = (Lift) Enum.Parse(typeof(Lift), kv.Key);
                    repMax[lift] = kv.Value;
                }
            }
        }

        public Dictionary<string, RepMax> Users => users;
        
        private Dictionary<string, RepMax> users = new Dictionary<string, RepMax>();
    }
}