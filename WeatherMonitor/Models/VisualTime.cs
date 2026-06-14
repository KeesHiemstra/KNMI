using CHi.Extensions;

using Newtonsoft.Json;

using System;
using System.ComponentModel;
using System.IO;
using System.Timers;

using Trinet.Core;

using Timer = System.Timers.Timer;

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
		private DateTime moonRiseTime;
		private DateTime moonSetTime;
		private string moonPhase;
		private string moonPhaseTooltip;

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
					CalculateMoonTime();
					MoonPhase();
				}
			}
		}

		internal GeographicLocation location;
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
		public DateTime MoonRiseTime
		{
			get => moonRiseTime;
			private set
			{
				if (MoonRiseTime != value)
				{
					moonRiseTime = value;
					NotifyPropertyChanged("MoonRiseTime");
				}
			}
		}
		public DateTime MoonSetTime
		{
			get => moonSetTime;
			private set
			{
				if (MoonSetTime != value)
				{
					moonSetTime = value;
					NotifyPropertyChanged("MoonSetTime");
				}
			}
		}
		public string Moonphase 
		{ 
			get => moonPhase;
			private set
			{
				if (Moonphase != value)
				{
					moonPhase = value;
					NotifyPropertyChanged("Moonphase");
				}
			}
		}
		public string MoonphaseTooltip
		{
			get => moonPhaseTooltip;
			private set
			{
				if (MoonphaseTooltip != value)
				{
					moonPhaseTooltip = value;
					NotifyPropertyChanged("MoonphaseTooltip");
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

			CreateTimer();
		}

		#endregion

		#region [ Public events ]

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

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

		/// <summary>
		/// Calculate the SunriseTime and SunsetTime.
		/// </summary>
		private void CalculateDayLight()
		{
			//Read the stored Open Weather location json
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

		/// <summary>
		/// Calculate the Moon rise and Moon set times.
		/// </summary>
		private void CalculateMoonTime()
		{
			MoonRise moonRise = new MoonRise();
			moonRise.Calculate(location.Latitude, location.Longitude, DateTime.Now);
			MoonRiseTime = moonRise.RiseTime;
			MoonSetTime = moonRise.SetTime;
		}

		private void MoonPhase()
		{
			MoonPhase moonPhase = new MoonPhase(DateTime.UtcNow);
			Moonphase = moonPhase.Visibility.ToString("0.0 % ") + moonPhase.Emoji;
			MoonphaseTooltip = $"Moon phase: {moonPhase.Name}\n" +
				$"Visibility: {moonPhase.Visibility:0.000 %}\n" +
				$"Emoji: {moonPhase.Emoji}";
		}
	
	}
}
