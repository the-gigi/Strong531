using System;
using System.Collections.Generic;
using System.IO;
using Strong531;
using YamlDotNet.Serialization;

namespace Strong531ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var cycleCount = args.Length > 0 ? int.Parse(args[0]) : 2; 
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var configFile = Path.Join(homeDir, ".Strong531/config.yaml");
            var config = new Configuration(configFile);
            var userPlans = new Dictionary<string, Plan>();
            
            foreach (KeyValuePair<string, RepMax> kv in config.Users)
            {
                var user = kv.Key;
                var repMax = kv.Value;
                var plan = PlanMaker.MakePlan(repMax, cycleCount);
                userPlans.Add(user, plan);
            }
            
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(userPlans);
            Console.Out.WriteLine(yaml);
        }
    }
}