using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Tsv.Service
{

    public class Config
    {
        private static string settingsFile = "settings.yml";
        private static Config _instance;
        public static Config Instance => _instance ?? (_instance = new Config());
        public Settings Settings { get; private set; }
        private Config()
        {
            var path = Path.Combine(Path.GetDirectoryName(typeof(Settings).Assembly.Location), settingsFile);
            var deserializer = new Deserializer();
            this.Settings = deserializer.Deserialize<Settings>(File.ReadAllText(path));
        }
    }

    public class Settings
    {
        [YamlMember(Alias = "groups", ApplyNamingConventions = false)]
        public List<Group> Groups { get; set; }

        [YamlMember(Alias = "indent", ApplyNamingConventions = false)]
        public double Indent { get; set; }

        [YamlMember(Alias = "enableLog", ApplyNamingConventions = false)]
        public bool EnableLog { get; set; } = true;

        [YamlMember(Alias = "roundDecimal", ApplyNamingConventions = false)]
        public int RoundDecimal { get; set; } 
    }

    public class Group
    {
        [YamlMember(Alias = "name", ApplyNamingConventions = false)]
        public string Name { get; set; }

        [YamlMember(Alias = "description", ApplyNamingConventions = false)]
        public string Description { get; set; }

        [YamlMember(Alias = "expression", ApplyNamingConventions = false)]
        public string Expression { get; set; }
    }
}
