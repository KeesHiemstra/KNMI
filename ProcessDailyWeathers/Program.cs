using CHi.Extensions;
using CHi.Log;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using WeatherDemon.Models;

namespace ProcessDailyWeathers
{
	class Program
	{
		static List<DayStatistics> DayStatistics = new List<DayStatistics>();

		static void Main(string[] args)
		{
			string StatisticsPath = @"%OneDrive%\Data\DailyWeather";
			OpenFolder(StatisticsPath.TranslatePath());
			SaveStatistics("DailyWeathers.json");

			Console.Write("\nPress any key...");
			Console.ReadKey();
		}

		private static void OpenFolder(string path)
		{
			if (!Directory.Exists(path))
			{
				Log.Write($"Folder '{path}' does not exist");
				Console.WriteLine($"Folder '{path}' does not exist");
				return;
			};

			IEnumerable<string> files = Directory.EnumerateFiles(path, "DayWeather_*.json");
			foreach (string file in files)
			{
				OpenFile(file);
			}
		}

		private static void OpenFile(string file)
		{
			List<DayWeather> dayWeather = new List<DayWeather>();
			string jsonPath = file; 
			using (StreamReader stream = File.OpenText(jsonPath))
			{
				string json = stream.ReadToEnd();
				dayWeather = JsonConvert.DeserializeObject <List<DayWeather>>(json);
			}
		
			ProcessDayWeather(dayWeather);
		}

		private static void ProcessDayWeather(List<DayWeather> dayWeather)
		{
			if (dayWeather[0].Time.Date == DateTime.Now.Date) { return; }

			DayStatistics statistics = new DayStatistics()
			{
				Date = dayWeather[0].Time.Date,
				FirstTemperature = dayWeather[0].Temperature,
				LastTempurature = dayWeather[dayWeather.Count - 1].Temperature,
				DiffTemperature = dayWeather[dayWeather.Count - 1].Temperature - dayWeather[0].Temperature,
				MinTemperature = dayWeather.Min(x => x.Temperature),
				MaxTemperature = dayWeather.Max(x => x.Temperature),
				AverageTemperature = dayWeather.Average(x => x.Temperature),
			};

			decimal variance = dayWeather.Sum(x => (x.Temperature - statistics.AverageTemperature) * (x.Temperature - statistics.AverageTemperature)) / dayWeather.Count;
			statistics.StandardDeviation = (decimal)Math.Sqrt((double)variance);

			DayStatistics.Add(statistics);
		}

		private static void SaveStatistics(string statisticsPath)
		{
			string json = JsonConvert.SerializeObject(DayStatistics, Formatting.Indented);
			using StreamWriter stream = new StreamWriter(statisticsPath);
			stream.Write(json);
		}

	}
}
