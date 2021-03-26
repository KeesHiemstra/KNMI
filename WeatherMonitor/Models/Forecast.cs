using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Models
{
	/// <summary>
	/// The information is manually entered data.
	/// </summary>
	public class Forecast
	{
		public DateTime MeasureDate { get; set; }
		public DateTime ForecastDate { get; set; }
		public string WeatherType { get; set; }
		public int MaxTemperature { get; set; }
		public int MinTemperature { get; set; }
	}
}
