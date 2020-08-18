using CHi.Extensions;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Models
{
	public class DayWeathers
	{

		#region [ Fields ]

		private readonly string DayWeatherJsonPath = "%OneDrive%\\Data\\DailyWeather".TranslatePath();

		#endregion

		#region [ Properties ]

		public List<DayWeather> Weathers { get; set; } = new List<DayWeather>();
		public DateTime Date { get; set; }
		public DateTime TimeLastWrite { get; set; }
		public DateTime TimeMin { get; set; }
		public DateTime TimeMax { get; set; }
		public decimal TemperatureMin { get; set; }
		public decimal TemperatureMax { get; set; }

		#endregion

		#region [ Construction ]

		public DayWeathers()
		{
			Date = DateTime.Now.Date;
			LoadDayWeather($"{DayWeatherJsonPath}\\DayWeather.json");
		}

		public DayWeathers(DateTime date)
		{
			Date = date.Date;
			LoadDayWeather($"{DayWeatherJsonPath}\\DayWeather_{date:yyyy-MM-dd}.json");
		}

		#endregion

		#region [ Public methods ]


		#endregion


		private void LoadDayWeather(string jsonFile)
		{
			if (File.Exists(jsonFile))
			{
				FileInfo info = new FileInfo(jsonFile);

				using (StreamReader stream = File.OpenText(jsonFile))
				{
					string json = stream.ReadToEnd();
					Weathers = JsonConvert.DeserializeObject<List<DayWeather>>(json)
						.Where(x => x.Time >= Date)
						.ToList();
				}

				TimeLastWrite = info.LastWriteTimeUtc;
			}

			TimeMin = Weathers.Min(x => x.Time);
			TimeMax = Weathers.Max(x => x.Time);
			TemperatureMin = Weathers.Min(x => x.Temperature);
			TemperatureMax = Weathers.Max(x => x.Temperature);
		}


	}
}
