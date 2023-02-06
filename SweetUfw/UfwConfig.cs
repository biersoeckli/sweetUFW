using System;
using System.Text.Json;

namespace SweetUfw
{
	public static class UfwConfig
	{
		private static string GetConfFilePath()
		{
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			return Path.Combine(Path.GetDirectoryName(strExeFilePath), "sweet.conf.json");
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// string => Hostname
		/// List<int> => allowed port numbers
		/// </returns>
		public static Dictionary<string, List<int>> Get(string? configPath = null)
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
				return new Dictionary<string, List<int>>();

            }
            return JsonSerializer.Deserialize<Dictionary<string, List<int>>>(jsonData) ?? new Dictionary<string, List<int>>();
		}

		public static void WriteConfig(Dictionary<string, List<int>> config, string? configPath = null)
		{
           var jsonData = JsonSerializer.Serialize(config);
			File.WriteAllText(configPath ?? GetConfFilePath(), jsonData);
        }
	}
}

