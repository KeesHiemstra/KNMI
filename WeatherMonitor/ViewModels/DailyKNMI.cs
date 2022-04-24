using CHi.Extensions;
using KNMI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherMonitor.ViewModels
{
	public class DailyKNMI : INotifyPropertyChanged
	{
		public const string dbConnection = @"Database=Weather;Data Source=(Local);Trusted_Connection=True;MultipleActiveResultSets=true";

		#region [ Fields ]

		public static WeatherDbContext Db { get; set; }
		private DateTime updateDate;

		#endregion

		#region [ Properties ]

		public DateTime UpdateDate
		{
			get => updateDate;
			set
			{
				if (value != updateDate)
				{
					updateDate = value;
					NotifyPropertyChanged("UpdateDate");
				}
			}
		}

		#endregion

		#region [ Constructor ]

		public DailyKNMI()
		{

			Log.Write("Initialize DailyKNMI");
			Db = new WeatherDbContext(dbConnection);
			InitialData();

		}

		#endregion

		#region [ Methods ]

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				Log.Write($"Property '{propertyName}' has been changed");
			}
		}
		#endregion

		#endregion

		private async Task InitialData()
		{

			string DownloadedKNMIStations;

			using (Db)
			{
				ExecuteUpdateDate();
				if (UpdateDate < DateTime.Now.Date.AddDays(-1))
				{
					Log.Write("DailyKNMI: Start download");

					DailyKNMIUpdate daily = new DailyKNMIUpdate(this, Db);
					DownloadedKNMIStations = daily.GetDownloadedKNMIStations();
					await daily.UpdateKNMIData(DownloadedKNMIStations, UpdateDate.AddDays(1));

					Log.Write("DailyKNMI: Finished download");
				}
			}

		}

		internal void ExecuteUpdateDate()
		{

			UpdateDate = GetUpdateDate();
			Log.Write($"DailyKNMI: UpdateDate: {UpdateDate.ToString("yyyy-MM-dd")}");

		}

		#region GetUpdateDate()
		/// <summary>
		/// Get the latest downloaded KNMI data.
		/// </summary>
		/// <returns></returns>
		private DateTime GetUpdateDate()
		{

			if (Db.IsDisposed())
			{
				Log.Write("DailyKNMI: Db was disposed");
				Db = new WeatherDbContext(dbConnection);
				Log.Write("DailyKNMI: Restarted Db");
			}

			try
			{
				var record = Db.Reports
					.OrderByDescending(x => x.Date)
					.Select(x => x.Date)
					.FirstOrDefault();

				return record;
			}
			catch (Exception ex)
			{
				Log.Write($"DailyKNMI: Exception GetUpdateDate: {ex.Message}");
				return DateTime.MinValue;
			}

		}
		#endregion

		#region GetDailyRange

		public List<DailyReport> GetDailyRange(int station, DateTime startDate, DateTime endDate)
		{

			if (Db.IsDisposed())
			{
				Log.Write("DailyKNMI: Db was disposed");
				Db = new WeatherDbContext(dbConnection);
				Log.Write("DailyKNMI: Restarted Db");
			}

			List<DailyReport> range = new List<DailyReport>();

			try
			{
				 range = Db.Reports
					.AsNoTracking()
					.Where(x => (x.Stn == station && x.Date >= startDate && x.Date <= endDate))
					.ToList();
			}
			catch (Exception ex)
			{
				Log.Write($"DailyKNMI: Exception GetDailyRange::record: {ex.Message}");
			}

			return range;

		}

		#endregion

	}
}
