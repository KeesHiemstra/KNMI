using System;
using System.Collections.Generic;

namespace WeatherDemon.Models
{
	public class DayWeather
	{
		public DateTime DemonTime { get; set; }
		public DateTime Time { get; set; }
		public decimal Temperature { get; set; }
		public int Pressure { get; set; }
		public int Humidity { get; set; }
		public int Visibility { get; set; }
		public decimal WindSpeed { get; set; }
		public int WindDirection { get; set; }
		public int Covering { get; set; }
		public int Condition { get; set; }

		public int WindSpeedBft 
		{
			get => CalculateWindSpeedInBeaufort();
		}

		public string DisplayWindDirection
		{
			get => CompileWindDirection();
		}

		public decimal DisplayCovering
		{
			get => (decimal)Covering / 100;
		}

		public string DisplayCondition
		{
			get => CompileCondition();
		}

		private string CompileWindDirection()
		{
			string[] DirectionName = new string[16]
			{
				"N", "NNO", "NO", "ONO",
				"O", "OZO", "ZO", "ZZO",
				"Z", "ZZW", "ZW", "WZW",
				"W", "WNW", "NW", "NNW"
			};
			int direction = (int)Math.Round(((float)WindDirection / 22.5));
			if (direction == 16) { direction = 0; }

			return $"{WindDirection} ({DirectionName[direction]})";
		}

		private int CalculateWindSpeedInBeaufort()
		{
			if (WindSpeed < 1) { return 0; }
			if (WindSpeed < 5) { return 1; }
			if (WindSpeed < 11) { return 2; }
			if (WindSpeed < 19) { return 3; }
			if (WindSpeed < 28) { return 4; }
			if (WindSpeed < 38) { return 5; }
			if (WindSpeed < 49) { return 6; }
			if (WindSpeed < 61) { return 7; }
			if (WindSpeed < 74) { return 8; }
			if (WindSpeed < 88) { return 9; }
			if (WindSpeed < 102) { return 10; }
			if (WindSpeed < 117) { return 11; }
			return 12;
		}

		private string CompileCondition()
		{
			Dictionary<int, string> condition = new Dictionary<int, string>
			{
				{ 200, "thunderstorm with light rain" },
				{ 201, "thunderstorm with rain" },
				{ 202, "thunderstorm with heavy rain" },
				{ 210, "light thunderstorm" },
				{ 211, "thunderstorm" },
				{ 212, "heavy thunderstorm" },
				{ 221, "ragged thunderstorm" },
				{ 230, "thunderstorm with light drizzle" },
				{ 231, "thunderstorm with drizzle" },
				{ 232, "thunderstorm with heavy drizzle" },
				{ 300, "light intensity drizzle" },
				{ 301, "drizzle" },
				{ 302, "heavy intensity drizzle" },
				{ 310, "light intensity drizzle rain" },
				{ 311, "drizzle rain" },
				{ 312, "heavy intensity drizzle rain" },
				{ 313, "shower rain and drizzle" },
				{ 314, "heavy shower rain and drizzle" },
				{ 321, "shower drizzle" },
				{ 500, "light rain" },
				{ 501, "moderate rain" },
				{ 502, "heavy intensity rain" },
				{ 503, "very heavy rain" },
				{ 504, "extreme rain" },
				{ 511, "freezing rain" },
				{ 520, "light intensity shower rain" },
				{ 521, "shower rain" },
				{ 522, "heavy intensity shower rain" },
				{ 531, "ragged shower rain" },
				{ 600, "light snow" },
				{ 601, "Snow" },
				{ 602, "Heavy snow" },
				{ 611, "Sleet" },
				{ 612, "Light shower sleet" },
				{ 613, "Shower sleet" },
				{ 615, "Light rain and snow" },
				{ 616, "Rain and snow" },
				{ 620, "Light shower snow" },
				{ 621, "Shower snow" },
				{ 622, "Heavy shower snow" },
				{ 701, "mist" },
				{ 711, "Smoke" },
				{ 721, "Haze" },
				{ 731, "sand/ dust whirls" },
				{ 741, "fog" },
				{ 751, "sand" },
				{ 761, "dust" },
				{ 762, "volcanic ash" },
				{ 771, "squalls" },
				{ 781, "tornado" },
				{ 800, "clear sky" },
				{ 801, "few clouds: 11 - 25 %" },
				{ 802, "scattered clouds: 25 - 50 %" },
				{ 803, "broken clouds: 51 - 84 %" },
				{ 804, "overcast clouds: 85 - 100 %" }
			};

			return condition[Condition];
		}
	}

}
