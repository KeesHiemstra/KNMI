using Newtonsoft.Json;

using WeatherDemon.Models;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MonthTemperatures
{
	class Program
	{
		public static List<decimal> Temp = new List<decimal>();

		static void Main(string[] args)
		{
			CollectJune2021();
		}

		private static void CollectJune2021()
		{
			string path = @"C:\Users\chi\OneDrive\Data\DailyWeather\";
			string[] files = Directory.GetFiles(path, "DayWeather_2021-06-*.json");

			foreach (string file in files)
			{
				ProcessFile(file);
			}
			ResultAvgTemp();
		}

		private static void ProcessFile(string file)
		{
			using (StreamReader stream = File.OpenText(file))
			{
				string json = stream.ReadToEnd();
				List<DayWeather> DayWeathers = JsonConvert.DeserializeObject<List<DayWeather>>(json);
				decimal AvgTemp = DayWeathers.Average(x => x.Temperature);
				AvgTemp = decimal.Round(AvgTemp, 1);
				Temp.Add(AvgTemp);
			}
		}

		private static void ResultAvgTemp()
		{
			decimal AvgTemp = decimal.Round(Temp.Average(x => x), 1);

			Console.WriteLine($"Gemiddelde maand temperatuur: {AvgTemp}");
		}
	}
}
