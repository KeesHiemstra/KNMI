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
		public decimal MinTemperature { get; set; }
		public decimal MaxTemperature { get; set; }

		public int WindSpeedBft 
		{
			get => CalculateWindSpeedInBeaufort();
		}

		public decimal DisplayHumidity
		{
			get => (decimal)Humidity / 100;
		}

		public string DisplayWindDirection
		{
			get => CompileWindDirection();
		}

		public decimal DisplayVisibility
		{
			get => (decimal)Visibility / 1000;
		}

		public decimal DisplayCovering
		{
			get => (decimal)Covering / 100;
		}

		public string DisplayCondition
		{
			get => CompileCondition();
		}

		public string DisplayMinMax
		{
			get => $"{MinTemperature}/{MaxTemperature}";
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
			Dictionary<int, string> condition = new Dictionary<int, string>();
			condition.Add(200, "thunderstorm with light rain");
			condition.Add(201, "thunderstorm with rain");
			condition.Add(202, "thunderstorm with heavy rain");
			condition.Add(210, "light thunderstorm");
			condition.Add(211, "thunderstorm");
			condition.Add(212, "heavy thunderstorm");
			condition.Add(221, "ragged thunderstorm");
			condition.Add(230, "thunderstorm with light drizzle");
			condition.Add(231, "thunderstorm with drizzle");
			condition.Add(232, "thunderstorm with heavy drizzle");
			condition.Add(300, "light intensity drizzle");
			condition.Add(301, "drizzle");
			condition.Add(302, "heavy intensity drizzle");
			condition.Add(310, "light intensity drizzle rain");
			condition.Add(311, "drizzle rain");
			condition.Add(312, "heavy intensity drizzle rain");
			condition.Add(313, "shower rain and drizzle");
			condition.Add(314, "heavy shower rain and drizzle");
			condition.Add(321, "shower drizzle");
			condition.Add(500, "light rain");
			condition.Add(501, "moderate rain");
			condition.Add(502, "heavy intensity rain");
			condition.Add(503, "very heavy rain");
			condition.Add(504, "extreme rain");
			condition.Add(511, "freezing rain");
			condition.Add(520, "light intensity shower rain");
			condition.Add(521, "shower rain");
			condition.Add(522, "heavy intensity shower rain");
			condition.Add(531, "ragged shower rain");
			condition.Add(600, "light snow");
			condition.Add(601, "Snow");
			condition.Add(602, "Heavy snow");
			condition.Add(611, "Sleet");
			condition.Add(612, "Light shower sleet");
			condition.Add(613, "Shower sleet");
			condition.Add(615, "Light rain and snow");
			condition.Add(616, "Rain and snow");
			condition.Add(620, "Light shower snow");
			condition.Add(621, "Shower snow");
			condition.Add(622, "Heavy shower snow");
			condition.Add(701, "mist");
			condition.Add(711, "Smoke");
			condition.Add(721, "Haze");
			condition.Add(731, "sand/ dust whirls");
			condition.Add(741, "fog");
			condition.Add(751, "sand");
			condition.Add(761, "dust");
			condition.Add(762, "volcanic ash");
			condition.Add(771, "squalls");
			condition.Add(781, "tornado");
			condition.Add(800, "clear sky");
			condition.Add(801, "few clouds: 11 - 25 %");
			condition.Add(802, "scattered clouds: 25 - 50 %");
			condition.Add(803, "broken clouds: 51 - 84 %");
			condition.Add(804, "overcast clouds: 85 - 100 %");

			return condition[Condition];
		}

		public string DisplayPessure
		{
			get
			{
				if (Pressure < 950) return "Veel te laag";
				if (Pressure < 967) return "Zware storm";
				if (Pressure < 980) return "Storm";
				if (Pressure < 993) return "Veel regen";
				if (Pressure < 1007) return "Regen of wind";
				if (Pressure < 1020) return "Veranderlijk weer";
				if (Pressure < 1033) return "Goed weer";
				if (Pressure < 1047) return "Mooi weer";
				if (Pressure < 1050 ) return "Zeer mooi weer";
				return "Veel te hoog";
			}
		}

	}

}
