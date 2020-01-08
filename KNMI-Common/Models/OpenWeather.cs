using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WeatherMonitor.Models
{
	[DataContract]
	public class OpenWeather
	{
		[DataMember]
		public Coord coord { get; set; }

		[DataMember]
		public List<WeatherI> weather { get; set; }

		[DataMember]
		public string @base { get; set; }

		[DataMember]
		public Main main { get; set; }

		[DataMember]
		public int visibility { get; set; }

		[DataMember]
		public Wind wind { get; set; }

		[DataMember]
		public Clouds clouds { get; set; }

		[DataMember]
		public int dt { get; set; }

		[DataMember]
		public Sys sys { get; set; }

		[DataMember]
		public int id { get; set; }

		[DataMember]
		public string name { get; set; }

		[DataMember]
		public int cod { get; set; }
	}

	[DataContract]
  public class Coord
  {
		[DataMember]
		public int lon { get; set; }

		[DataMember]
		public double lat { get; set; }
	}


	[DataContract]
	public class WeatherI
	{
		[DataMember]
		public int id { get; set; }

		[DataMember]
		public string main { get; set; }

		[DataMember]
		public string description { get; set; }

		[DataMember]
		public string icon { get; set; }
	}


	[DataContract]
	public class Main
	{
		[DataMember]
		public double temp { get; set; }

		[DataMember]
		public int pressure { get; set; }

		[DataMember]
		public int humidity { get; set; }

		[DataMember]
		public double temp_min { get; set; }

		[DataMember]
		public double temp_max { get; set; }
	}


	[DataContract]
	public class Wind
	{
		[DataMember]
		public double speed { get; set; }

		[DataMember]
		public int deg { get; set; }
	}


	[DataContract]
	public class Clouds
	{
		[DataMember]
		public int all { get; set; }
	}


	[DataContract]
	public class Sys
	{
		[DataMember]
		public int type { get; set; }

		[DataMember]
		public int id { get; set; }

		[DataMember]
		public double message { get; set; }

		[DataMember]
		public string country { get; set; }

		[DataMember]
		public int sunrise { get; set; }

		[DataMember]
		public int sunset { get; set; }
	}

}
