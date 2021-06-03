using KNMI.Models;

using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherMonitor.ViewModels
{
	public class DailyKNMIUpdate
	{

		internal DailyKNMI Daily;
		public static WeatherDbContext Db { get; set; }

		public DailyKNMIUpdate(DailyKNMI daily, WeatherDbContext db)
		{
			Daily = daily;
			Db = db;
		}

		/// <summary>
		/// Get the downloaded KNMI stations.
		/// Use these stations to update the data.
		/// </summary>
		/// <returns></returns>
		public string GetDownloadedKNMIStations()
		{

			var record = Db.Reports
				.AsNoTracking()
				.Select(x => x.Stn)
				.Distinct()
				.ToArray();

			string stations = string.Empty;
			foreach (var item in record)
			{
				if (!string.IsNullOrEmpty(stations))
				{
					stations += ":";
				}
				stations += item;
			}

			return stations;

		}

		/// <summary>
		/// Update the KNMI_Daily table.
		/// </summary>
		/// <param name="downloadedKNMIStations"></param>
		/// <param name="updateDate"></param>
		public async Task UpdateKNMIData(string downloadedKNMIStations, DateTime updateDate)
		{

			Log.Write("Start UpdateKNMIData");
			string data = await DownloadKNMIData(downloadedKNMIStations, updateDate);
			string[] header = GetDownloadHeader(data);
			if (header == null)
			{
				Log.Write("Downloaded data was incorrect");
				return;
			}

			using (Db)
			{
				await ProcessData(data, header);
			}
			Daily.ExecuteUpdateDate();
			Log.Write("Finished UpdateKNMIData");

		}

		/// <summary>
		/// Download the KNMI data filtered on stations and the date.
		/// </summary>
		/// <param name="stations"></param>
		/// <param name="date"></param>
		/// <returns></returns>
		private static async Task<string> DownloadKNMIData(string stations, DateTime date)
		{

			//const string url = "http://projects.knmi.nl/klimatologie/daggegevens/getdata_dag.cgi";
			const string url = "https://www.daggegevens.knmi.nl/klimatologie/daggegevens";

			HttpClient http = new HttpClient();
			HttpResponseMessage httpRespond =
				await http.GetAsync($"{url}?" +
					$"stns={stations}&" +
					$"start={date.ToString("yyyyMMdd")}&" +
					$"end={DateTime.Now.ToString("yyyyMMdd")}");

			Log.Write($"Url: {url}?stns={stations}&start={date.ToString("yyyyMMdd")}");
			Log.Write($"Respond statusCode: {httpRespond.StatusCode}");
			
			string httpResult = await httpRespond.Content.ReadAsStringAsync();
			Log.Write($"Downloaded {httpResult.Length} bytes");

			return httpResult;

		}

		/// <summary>
		/// Get the header from the downloaded data.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static string[] GetDownloadHeader(string data)
		{
			if (data.StartsWith("<!DOCTYPE html>"))
			{
				//The downloaded data was incorrect
				return null;
			}
			Log.Write($"GetDownloadHeader from {data.Length} downloaded bytes");
			//Split the downloaded data in line and skip empty lines.
			string[] lines = data.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

			//Search for the header
			string header = lines
				.Where(x => x.StartsWith("# STN,YYYYMMDD"))
				.FirstOrDefault();

			header = header.Replace("# ", "");

			//Get the cleaned fields.
			string[] fields = header.Split(',');
			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = fields[i].Trim();
			}

			return fields;

		}

		/// <summary>
		/// Process downloaded data.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="header"></param>
		private static async Task ProcessData(string data, string[] header)
		{

			Log.Write($"ProcessData from {data.Length} bytes with {header.Length} fields");
			int recordCount = 0;
			//Split the downloaded data in line and skip empty lines.
			string[] lines = data.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string line in lines)
			{
				//Skip the header
				if (line.StartsWith("#"))
				{
					continue;
				}

				string[] fields = line.Split(',');
				recordCount = await ProcessLine(header, fields, recordCount);
			}
			Log.Write($"Processed {recordCount} lines");

		}

		/// <summary>
		/// Process the data per line and save the data to the database.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="fields"></param>
		private static async Task<int> ProcessLine(string[] header, string[] fields, int recordCount)
		{

			DailyReport newRecord = new DailyReport();

			//Map the header field to DailyReport record properties.

			for (int i = 0; i < header.Length; i++)
			{
				//The provided download example had a different order of the fields.
				#region Import
				switch (header[i].ToLower())
				{
					//STN      = Station number.
					case "stn":
						newRecord.Stn = int.Parse(fields[i]);
						break;
					//YYYYMMDD = Date (YYYY=year MM=month DD=day).
					case "yyyymmdd":
						newRecord.Date = TranslateDate(fields[i]);
						break;
					//DDVEC    = Vector mean wind direction in degrees (360=north, 90=east, 180=south, 270=west, 0=calm/variable).
					case "ddvec":
						newRecord.DDVec = TranslateInt(fields[i]);
						break;
					//FHVEC    = Vector mean wind speed (in 0.1 m/s).
					case "fhvec":
						newRecord.FHVec = Translatedecimal(fields[i]);
						break;
					//FG       = Daily mean wind speed (in 0.1 m/s).
					case "fg":
						newRecord.FG = Translatedecimal(fields[i]);
						break;
					//FHX      = Maximum hourly mean wind speed (in 0.1 m/s).
					case "fhx":
						newRecord.FHX = Translatedecimal(fields[i]);
						break;
					//FHXH     = Hourly division in which FHX was measured.
					case "fhxh":
						newRecord.FHXH = TranslateInt(fields[i]);
						break;
					//FHN      = Minimum hourly mean wind speed (in 0.1 m/s).
					case "fhn":
						newRecord.FHN = Translatedecimal(fields[i]);
						break;
					//FHNH     = Hourly division in which FHN was measured.
					case "fhnh":
						newRecord.FHNH = TranslateInt(fields[i]);
						break;
					//FXX      = Maximum wind gust (in 0.1 m/s).
					case "fxx":
						newRecord.FXX = Translatedecimal(fields[i]);
						break;
					//FXXH     = Hourly division in which FXX was measured.
					case "fxxh":
						newRecord.FXXH = TranslateInt(fields[i]);
						break;
					//TG       = Daily mean temperature in (0.1 degrees Celsius).
					case "tg":
						newRecord.TG = Translatedecimal(fields[i]);
						break;
					//TN       = Minimum temperature (in 0.1 degrees Celsius).
					case "tn":
						newRecord.TN = Translatedecimal(fields[i]);
						break;
					//TNH      = Hourly division in which TN was measured.
					case "tnh":
						newRecord.TNH = TranslateInt(fields[i]);
						break;
					//TX       = Maximum temperature (in 0.1 degrees Celsius).
					case "tx":
						newRecord.TX = Translatedecimal(fields[i]);
						break;
					//TXH      = Hourly division in which TX was measured.
					case "txh":
						newRecord.TXH = TranslateInt(fields[i]);
						break;
					//T10N     = Minimum temperature at 10 cm above surface (in 0.1 degrees Celsius).
					case "t10n":
						newRecord.T10N = Translatedecimal(fields[i]);
						break;
					//T10NH    = 6-hourly division in which T10N was measured. 6=0-6 UT, 12=6-12 UT, 18=12-18 UT, 24=18-24 UT
					case "t10nh":
						newRecord.T10NH = TranslateInt(fields[i]);
						break;
					//SQ       = Sunshine duration (in 0.1 hour) calculated from global radiation (-1 for <0.05 hour).
					case "sq":
						newRecord.SQ = TranslateDecimal(fields[i]);
						break;
					//SP       = Percentage of maximum potential sunshine duration.
					case "sp":
						newRecord.SP = TranslateInt(fields[i]);
						break;
					//Q        = Global radiation (in J/cm2).
					case "q":
						newRecord.Q = TranslateInt(fields[i]);
						break;
					//DR       = Precipitation duration (in 0.1 hour).
					case "dr":
						newRecord.DR = Translatedecimal(fields[i]);
						break;
					//RH       = Daily precipitation amount (in 0.1 mm) (-1 for <0.05 mm).
					case "rh":
						newRecord.RH = Translatedecimal(fields[i]);
						break;
					//RHX      = Maximum hourly precipitation amount (in 0.1 mm) (-1 for <0.05 mm).
					case "rhx":
						newRecord.RHX = Translatedecimal(fields[i]);
						break;
					//RHXH     = Hourly division in which RHX was measured.
					case "rhxh":
						newRecord.RHXH = TranslateInt(fields[i]);
						break;
					//PG       = Daily mean sea level pressure (in 0.1 hPa) calculated from 24 hourly values.
					case "pg":
						newRecord.PG = Translatedecimal(fields[i]);
						break;
					//PX       = Maximum hourly sea level pressure (in 0.1 hPa).
					case "px":
						newRecord.PX = Translatedecimal(fields[i]);
						break;
					//PXH      = Hourly division in which PX was measured.
					case "pxh":
						newRecord.PXH = TranslateInt(fields[i]);
						break;
					//PN       = Minimum hourly sea level pressure (in 0.1 hPa).
					case "pn":
						newRecord.PN = Translatedecimal(fields[i]);
						break;
					//PNH      = Hourly division in which PN was measured.
					case "pnh":
						newRecord.PNH = TranslateInt(fields[i]);
						break;
					//VVN      = Minimum visibility. 0: <100 m, 1:100-200 m, 2:200-300 m,..., 49:4900-5000 m, 50:5-6 km, 56:6-7 km, 57:7-8 km,..., 79:29-30 km, 80:30-35 km, 81:35-40 km,..., 89: >70 km)
					case "vvn":
						newRecord.VVN = TranslateInt(fields[i]);
						break;
					//VVNH     = Hourly division in which VVN was measured.
					case "vvnh":
						newRecord.VVNH = TranslateInt(fields[i]);
						break;
					//VVX      = Maximum visibility. 0: <100 m, 1:100-200 m, 2:200-300 m,..., 49:4900-5000 m, 50:5-6 km, 56:6-7 km, 57:7-8 km,..., 79:29-30 km, 80:30-35 km, 81:35-40 km,..., 89: >70 km)
					case "vvx":
						newRecord.VVX = TranslateInt(fields[i]);
						break;
					//VVXH     = Hourly division in which VVX was measured.
					case "vvxh":
						newRecord.VVXH = TranslateInt(fields[i]);
						break;
					//NG       = Mean daily cloud cover (in octants, 9=sky invisible).
					case "ng":
						newRecord.NG = TranslateInt(fields[i]);
						break;
					//UG       = Daily mean relative atmospheric humidity (in percents).
					case "ug":
						newRecord.UG = TranslateInt(fields[i]);
						break;
					//UX       = Maximum relative atmospheric humidity (in percents).
					case "ux":
						newRecord.UX = TranslateInt(fields[i]);
						break;
					//UXH      = Hourly division in which UX was measured.
					case "uxh":
						newRecord.UXH = TranslateInt(fields[i]);
						break;
					//UN       = Minimum relative atmospheric humidity (in percents).
					case "un":
						newRecord.UN = TranslateInt(fields[i]);
						break;
					//UNH      = Hourly division in which UN was measured.
					case "unh":
						newRecord.UNH = TranslateInt(fields[i]);
						break;
					//EV24     = Potential evapotranspiration (Makkink) (in 0.1 mm).
					case "ev24":
						newRecord.EV24 = Translatedecimal(fields[i]);
						break;
					default:
						break;
				}
				#endregion
			}

			//Save the record in the database.
			var record = await Db.Reports
				.AsNoTracking()
				.Where(x => (x.Stn == newRecord.Stn && x.Date == newRecord.Date))
				.CountAsync();

			if (record == 0)
			{
				Db.Reports.Add(newRecord);
				Db.SaveChanges();
				recordCount++;
			}

			return recordCount;

		}

		/// <summary>
		/// Translate to date.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static DateTime TranslateDate(string str)
		{

			// Format: YYYYMMDD => yyyy-MM-dd
			str = str.Insert(6, "-").Insert(4, "-");
			return DateTime.Parse(str);

		}

		/// <summary>
		/// Translate to integer. It might be null.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static int? TranslateInt(string str)
		{

			int? data = null;

			if (!string.IsNullOrWhiteSpace(str))
			{
				data = int.Parse(str);
			}

			return data;

		}

		/// <summary>
		/// Translate to decimal and divide to 10. It might be null.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static decimal? TranslateDecimal(string str)
		{

			decimal? data = null;

			if (!string.IsNullOrWhiteSpace(str))
			{
				data = decimal.Parse(str) / 10;
			}

			return data;

		}

		/// <summary>
		/// Translate to decimal and divide to 10. It might be null.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static decimal? Translatedecimal(string str)
		{

			decimal? data = null;

			if (!string.IsNullOrWhiteSpace(str))
			{
				data = decimal.Parse(str) / 10;
			}

			return data;

		}

	}
}
