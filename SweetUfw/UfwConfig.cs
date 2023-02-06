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
		public static Dictionary<string, List<int>> Get()
		{
			var jsonData = File.ReadAllText(GetConfFilePath());
			if (jsonData is null)
			{
				return new Dictionary<string, List<int>>();

            }
            return JsonSerializer.Deserialize<Dictionary<string, List<int>>>(jsonData) ?? new Dictionary<string, List<int>>();
		}

		public static void WriteConfig(Dictionary<string, List<int>> config)
		{
           var jsonData = JsonSerializer.Serialize(config);
			File.WriteAllText(GetConfFilePath(), jsonData);
        }
	}
}

