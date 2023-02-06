using System.Text.Json;

namespace SweetUfw
{
    public static class UfwConfig
    {
        private static string GetConfFilePath()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.Combine(Path.GetDirectoryName(strExeFilePath) ?? throw new FileNotFoundException($"Could not determine config path location {strExeFilePath}"),
                "sweet.conf.json");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// string => Hostname
        /// List<int> => allowed port numbers
        /// </returns>
        public static Dictionary<int, List<string>> Get(string? configPath = null)
        {
            string path = configPath ?? GetConfFilePath();
            if (!Path.Exists(path))
            {
                throw new FileNotFoundException($"The config file at '{path}' could not be found.");
            }
            Console.WriteLine($"Loading config from {path}");
            var jsonData = File.ReadAllText(path);
            if (jsonData is null)
            {
                return new Dictionary<int, List<string>>();

            }
            return JsonSerializer.Deserialize<Dictionary<int, List<string>>>(jsonData) ?? new Dictionary<int, List<string>>();
        }

        public static void WriteConfig(Dictionary<int, List<string>> config, string? configPath = null)
        {
            var jsonData = JsonSerializer.Serialize(config);
            File.WriteAllText(configPath ?? GetConfFilePath(), jsonData);
        }
    }
}

