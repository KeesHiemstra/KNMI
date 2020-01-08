using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherMonitor.Extensions
{
  public static class WeatherExtension
  {
		public static DateTime ConvertUnixTimeToDate(this int unixTime)
		{
			// Unix time stamp is seconds past epoch
			System.DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			return dateTime.AddSeconds(unixTime).ToLocalTime();
		}

	}
}
