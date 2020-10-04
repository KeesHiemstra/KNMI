using CHi.Extensions;

using Newtonsoft.Json;

using System;
using System.ComponentModel;
using System.IO;
#if WEATHERMONITOR
using System.Timers;
#endif

using Trinet.Core;

namespace WeatherMonitor.Models
{
	/// <summary>
	/// The property CurrentTime is automatically updated every second.
	/// INotifyPropertyChanged is implemented to update the window.
	/// This is public to access this class though the xaml part.
	/// </summary>
	public class VisualTime : INotifyPropertyChanged
	{
		// Updated the simple implementation to act on changes CurrentTime 
		// with INotifyPropertyChanged. 

		#region [ Fields ]

		private DateTime currentTime;
		private DateTime today;
		private DateTime sunriseTime;
		private DateTime sunsetTime;

		#endregion

		#region [ Properties ]

		public DateTime CurrentTime
		{
			get => currentTime;
			// It can not change outside this class.
			private set
			{
				if (currentTime != value)
				{
					currentTime = value;

					if (currentTime.Date >= Today)
					{
						Today = DateTime.Now.Date;
					}
					// Changed the property to update all properties.
					//NotifyPropertyChanged("CurrentTime");
					NotifyPropertyChanged("");
				}
			}
		}
		public DateTime Today
		{
			get => today.Date;
			private set
			{
				if (today != value)
				{
					today = value.Date;
					NotifyPropertyChanged("Today");
					CalculateDayLight();
				}
			}
		}
		public DateTime SunriseTime 
		{ 
			get => sunriseTime;
			private set 
			{
				if (sunriseTime != value)
				{
					sunriseTime = value;
					NotifyPropertyChanged("SunriseTime");
				}
			}
		}
		public DateTime SunsetTime 
		{ 
			get => sunsetTime;
			private set
			{
				if (sunsetTime != value)
				{
					sunsetTime = value;
					NotifyPropertyChanged("SunsetTime");
				}
			}
		}

		public string DisplayDate { get => CurrentTime.ToString("yyyy-MM-dd"); }
		public string DisplayTime { get => CurrentTime.ToString("HH:mm"); }
		public string DisplayTimeEx { get => CurrentTime.ToString("HH:mm:ss"); }
		public bool IsDaylight { get => CurrentTime >= SunriseTime && CurrentTime <= SunsetTime; }

		#endregion

		#region [ Constructions ]

		public VisualTime()
		{
			Today = DateTime.Now.Date;

#if WEATHERMONITOR
			CreateTimer();
#else
			CurrentTime = DateTime.Now;
#endif
		}

		public VisualTime(DateTime today)
		{
			Today = today.Date;
		}

		#endregion

		#region [ Public events ]

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Notification
		/// <summary>
		/// The NotifyPropertyChanged came with the implementation of the INotifyPropertyChanged 
		/// interface.
		/// </summary>
		/// <param name="propertyName">Limit the properties to this name. All properties are updated
		/// if the name is empty.</param>
		private void NotifyPropertyChanged(string propertyName = "") =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		// The origin snippet was helpful to add an extra action.
		//private void NotifyPropertyChanged(string propertyName = "")
		//{
		//	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		//	Trace.WriteLine($"{DateTime.Now:HH:mm:ss:fffffff} Current time is changed to {CurrentTime}");
		//}
		#endregion

		#region WeatherMonitor
#if WEATHERMONITOR
		/// <summary>
		/// Create the currentTimer and initialize the CurrentTime.
		/// </summary>
		private void CreateTimer()
		{
			CurrentTime = DateTime.Now;
			Timer currentTimer = new Timer()
			{
				Enabled = true,
				// The internal to every second was help to the update every minute.
				Interval = 1000
			};
			currentTimer.Elapsed += CurrentTimer_Elapsed;
		}

		/// <summary>
		/// Update the CurrentTime.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CurrentTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			CurrentTime = DateTime.Now;
		}
#endif
		#endregion

		/// <summary>
		/// Calculate the SunriseTime and SunsetTime.
		/// </summary>
		private void CalculateDayLight()
		{
			//Read the stored Open Weather location json
			GeographicLocation location;
			string jsonPath = "%OneDrive%\\Etc\\DemonOpenWeather.json".TranslatePath();
			using (StreamReader stream = File.OpenText(jsonPath))
			{
				string json = stream.ReadToEnd();
				location = JsonConvert.DeserializeObject<GeographicLocation>(json);
			}

			DateTime date = Today.Date.AddHours(2);

			//Calculate the daylight times
			DaylightHours daylight = DaylightHours.Calculate(date, location);
			SunriseTime = daylight.SunriseUtc.Value.LocalDateTime.ToLocalTime();
			SunsetTime = daylight.SunsetUtc.Value.LocalDateTime.ToLocalTime();
		}

	}
}
