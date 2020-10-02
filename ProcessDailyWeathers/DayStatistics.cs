using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessDailyWeathers
{
	class DayStatistics
	{
		public DateTime Date { get; set; }
		public decimal FirstTemperature { get; set; }
		public decimal LastTempurature { get; set; }
		public decimal DiffTemperature { get; set; }
		public decimal MinTemperature { get; set; }
		public decimal MaxTemperature { get; set; }
		public decimal AverageTemperature { get; set; }
		public decimal StandardDeviation { get; set; }
	}
}
