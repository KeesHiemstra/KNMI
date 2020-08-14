using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;

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

		#endregion

		#region [ Properties ]

		//public DateTime CurrentTime { get; private set; }
		public DateTime CurrentTime
		{
			get => currentTime;
			// It can not change outside this class.
			private set
			{
				if (currentTime != value)
				{
					currentTime = value;
					// Changed the property to update all properties.
					//NotifyPropertyChanged("CurrentTime");
					NotifyPropertyChanged("");
				}
			}
		}

		public string DisplayDate { get => CurrentTime.ToString("yyyy-MM-dd"); }
		public string DisplayTime { get => CurrentTime.ToString("HH:mm"); }
		public string DisplayTimeEx { get => CurrentTime.ToString("HH:mm:ss"); }

		#endregion

		#region [ Constructions ]

		public VisualTime()
		{
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
	}
}
