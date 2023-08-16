using CHi.Extensions;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using WeatherDemon.Models;

using WeatherMonitor.ViewModels;

namespace WeatherMonitor.Models
{
	public class DayWeathers : INotifyPropertyChanged
	{

		#region [ Fields ]
		//private MainWindow View;

		private readonly string DayWeatherJsonPath = "%OneDrive%\\Data\\DailyWeather".TranslatePath();
		private readonly string JsonFile;
#if WEATHERMONITOR
		private AutoResetEvent AutoEvent;
		private Timer AutoLoad;
		private int DelayAutoLoad = 10;
		private bool IsEvaluateDelay = false;
#endif
		private DateTime timeLastWrite;

		#endregion

		#region [ Properties ]

		public ObservableCollection<DayWeather> Weathers { get; set; } =
			new ObservableCollection<DayWeather>();
		public DateTime Date { get; set; }
		public DateTime TimeLastWrite
		{
			get => timeLastWrite;
			set
			{
				if (timeLastWrite != value)
				{
					timeLastWrite = value;
					NotifyPropertyChanged("");
				}
			}
		}
		public DateTime TimeLastCurrent { get; private set; }
		public DateTime TimeMin { get; private set; }
		public DateTime TimeMax { get; private set; }
		public decimal TemperatureCurrent { get; private set; }
		public decimal TemperatureMin { get; private set; }
		public decimal TemperatureMax { get; private set; }
		public int PressureMin { get; private set; }
		public int PressureMax { get; private set; }

		#endregion

		#region [ Construction ]

		public DayWeathers()
		{
			Log.Write("DayWeather: Load CurrentWeathers");
			Date = DateTime.Now.Date;
			JsonFile = $"{DayWeatherJsonPath}\\DayWeather.json";
			LoadDayWeather(JsonFile, true);

#if WEATHERMONITOR
			//Start only the timer with current weather
			StartTimer();
#endif
		}

		public DayWeathers(DateTime date)
		{
			Log.Write("DayWeather: Load PreviousWeathers");
			Date = date.Date;
			LoadDayWeather($"{DayWeatherJsonPath}\\DayWeather_{date:yyyy-MM-dd}.json", false);
		}

		#endregion

		#region [ Public methods ]

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		private async void LoadDayWeather(string jsonFile, bool isCurrentDay)
		{
			Log.Write("DayWeather: Start loading Weathers");

			List<DayWeather> weathers = new List<DayWeather>();
			if (File.Exists(jsonFile))
			{
				FileInfo info = new FileInfo(jsonFile);

				if (info.LastWriteTimeUtc > TimeLastWrite)
				{
#if WEATHERMONITOR
					if (IsEvaluateDelay)
					{
						Log.Write($"DayWeather: Passed evaluate delay period, delay: {DelayAutoLoad} seconds");
						AutoLoad.Change(new TimeSpan(0, 10, 0), new TimeSpan(0, 10, 0));
						IsEvaluateDelay = false;
					}
#endif

					Log.Write($"DayWeather: Open Weathers json '{jsonFile}'");
					using (StreamReader stream = File.OpenText(jsonFile))
					{
						string json = string.Empty;
						try
						{
							json = stream.ReadToEnd();
						}
						catch (Exception ex)
						{
							json = DayWeatherFileException(jsonFile,
								"DayWeather",
								$"DayWeather exception: '{jsonFile}': {ex.Message}");
						}
						weathers = JsonConvert.DeserializeObject<List<DayWeather>>(json)
							.Where(x => x.Time >= Date)
							.ToList();
						Weathers = new ObservableCollection<DayWeather>(weathers);
						Log.Write($"DayWeather: Loaded {Weathers.Count} records");
					}

					ProcessInfo(isCurrentDay);
					TimeLastWrite = info.LastWriteTimeUtc;
					Log.Write($"DayWeather: TimeLastWrite {TimeLastWrite}");

#if WEATHERMONITOR
					Log.Write("DayWeather: Loaded current Weathers file");
#endif
				}
				else
				{
#if WEATHERMONITOR
					Log.Write($"DayWeather: Evaluate delay: Added 10 second");
					AutoLoad.Change(new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 10));
					DelayAutoLoad += 10;
					IsEvaluateDelay = true;
#endif
				}
			}
		}

		private void ProcessInfo(bool isCurrentDay)
		{
			TimeMin = Weathers.Min(x => x.Time);
			TimeMax = Weathers.Max(x => x.Time);

			TemperatureMin = Weathers.Min(x => x.Temperature);
			TemperatureMax = Weathers.Max(x => x.Temperature);

			PressureMin = Weathers.Min(x => x.Pressure);
			PressureMax = Weathers.Max( x => x.Pressure);
		}

#if WEATHERMONITOR
		private void StartTimer()
		{
			TimeSpan start = TimeLastWrite.AddMinutes(10).AddSeconds(DelayAutoLoad) -
				DateTime.Now.ToUniversalTime();
			TimeSpan time = new TimeSpan(0, 10, 0);

			AutoEvent = new AutoResetEvent(false);
			AutoLoad = new Timer(CheckFile, AutoEvent, start, time);
			Log.Write($"DayWeather: CheckFile will be started at {DateTime.Now + start}");
		}

		private void CheckFile(Object stateInfo)
		{
			LoadDayWeather(JsonFile, true);
		}
#endif

		private string DayWeatherFileException(string file, string header, string message)
		{
			string result = string.Empty;
			Log.Write(message);

			if (File.Exists(file))
			{
				FileAttributes attr = File.GetAttributes(file);
				if ((int)attr >= 0x00080000)
				{
					//Force download a OneDrive
					Log.Write($"{header}: Pin {file}");
					attr += 0x00080000;
					File.SetAttributes(file, attr);
					Thread.Sleep(1500);
					try
					{
						using StreamReader stream = File.OpenText(file);
						result = stream.ReadToEnd();
					}
					catch (Exception ex)
					{
						MessageBox.Show($"{header}: Error reading '{file}' - {ex.Message}",
							"Error access file",
							MessageBoxButton.OK,
							MessageBoxImage.Error);
					}
				}
			}
			else
			{
				Log.Write($"{header}: '{file}' doesn't exists.");
			}

			return result;
		}
	}
}
