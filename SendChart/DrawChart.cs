using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WeatherDemon.Models;
using WeatherMonitor.Models;

namespace WeatherMonitor.ViewModels
{
	public class DrawChart
	{

		#region [ Fields ]

#if WEATHERMONITOR
		readonly MainViewModel VM;
#endif

		//Temperature graph area
		double MarginLeft;
		double MarginTop;
		double MarginBottom;

		//TimeLine (X axis)
		double TimeLineStr;
		double TimeLineFin;
		double TimeLineScale;

		//TemperatureLine (Y axis)
		double TempLineStr;
		double TempLineFin;
		double TempLineScale;

		//TimeLine time points
		DateTime DateStr;
		DateTime DateFin;

		decimal TempMin;
		decimal TempMax;

		#endregion

		#region [ Properties ]

		public Canvas Graph { get; set; } = new Canvas()
		{
			Background = Brushes.LightGray,
			Height = 250,
			Width = 400,
		};

		public DayWeathers CurrentWeathers { get; set; }
		public DayWeathers PreviousWeathers { get; set; }
		public VisualTime Now { get; } = new VisualTime();

		#endregion

		#region [ Construction ]

#if WEATHERMONITOR
		public DrawChart(MainViewModel mainVM)
#else
		public DrawChart(DayWeathers currentWeathers, DayWeathers previousWeathers, DateTime dateTime)
#endif
		{
#if WEATHERMONITOR
			VM = mainVM;
#else
			CurrentWeathers = currentWeathers;
			PreviousWeathers = previousWeathers;
#endif

			CalculateConstants();
			AddDaylight();
			AddAxis();
			AddGraphTitle();
			AddTemperatures();
			AddTemperaturesMilestones();
		}

#endregion

#region [ Public methods ]


#endregion

		/// <summary>
		/// Calculate beginning and ending values.
		/// </summary>
		private void CalculateConstants()
		{
			MarginLeft = 20;
			MarginTop = 30;
			MarginBottom = 20;

			TimeLineStr = MarginLeft;
			TimeLineFin = Graph.Width;

			TempLineStr = Graph.Height - MarginBottom;
			TempLineFin = MarginTop;

			DateStr = CurrentWeathers.Date;
			DateFin = DateStr.AddDays(1).AddSeconds(-1);

			TimeLineScale = (TimeLineFin - TimeLineStr) / (int)(DateFin - DateStr).TotalMinutes;

			TempMin = Math.Floor(PreviousWeathers.TemperatureMin - (decimal)0.25);
			if (CurrentWeathers.TemperatureMin < PreviousWeathers.TemperatureMin)
			{
				TempMin = Math.Floor(CurrentWeathers.TemperatureMin - (decimal)0.25);
			}
			TempMax = Math.Ceiling(PreviousWeathers.TemperatureMax + (decimal)0.25);
			if (CurrentWeathers.TemperatureMax > PreviousWeathers.TemperatureMax)
			{
				TempMax = Math.Ceiling(CurrentWeathers.TemperatureMax + (decimal)0.25);
			}
			TempLineScale = (TempLineStr - TempLineFin) / (double)(TempMax - TempMin);
		}

		private void AddDaylight()
		{
			//Temperature graph area
			Rectangle area = new Rectangle()
			{
				Width = Graph.Width - MarginLeft,
				Height = Graph.Height - MarginTop - MarginBottom,
				Fill = Brushes.LightSlateGray,
			};
			Graph.Children.Add(area);
			Canvas.SetLeft(area, MarginLeft);
			Canvas.SetTop(area, MarginTop);

			//Draw the daylight times
			area = new Rectangle()
			{
				Width = ((Now.SunsetTime - Now.SunriseTime).TotalMinutes * TimeLineScale),
				Height = TempLineStr - TempLineFin,
				Fill = Brushes.LightCyan,
			};
			Graph.Children.Add(area);
			Canvas.SetLeft(area, TimeLineStr + (Now.SunriseTime - DateStr).TotalMinutes * TimeLineScale);
			Canvas.SetTop(area, MarginTop);
		}

		private void AddAxis()
		{
			//Time axis (X axis)
			Graph.Children.Add(new Line()
			{
				X1 = TimeLineStr,
				Y1 = Graph.Height - MarginBottom,
				X2 = TimeLineFin,
				Y2 = Graph.Height - MarginBottom,
				StrokeThickness = 1,
				Stroke = Brushes.Blue,
			});

			//Time scale (hour)
			DateTime time = DateStr.AddMinutes(60 - DateStr.Minute);
			while (time < DateFin)
			{
				Graph.Children.Add(new Line()
				{
					X1 = TimeLineStr + (time - DateStr).TotalMinutes * TimeLineScale,
					Y1 = Graph.Height - MarginBottom,
					X2 = TimeLineStr + (time - DateStr).TotalMinutes * TimeLineScale,
					Y2 = Graph.Height - MarginBottom + 3,
					StrokeThickness = 1,
					Stroke = Brushes.Blue,
				});

				if (time.Hour % 6 == 0)
				{
					TextBlock text = new TextBlock()
					{
						Text = time.Hour.ToString(),
						FontSize = 9,
					};
					Graph.Children.Add(text);
					Canvas.SetLeft(text, TimeLineStr + (time - DateStr).TotalMinutes * TimeLineScale - 2);
					Canvas.SetTop(text, TempLineStr + 7);
				}

				time = time.AddHours(1);
			}

			//Temperature axis (Y axis)
			Graph.Children.Add(new Line()
			{
				X1 = MarginLeft,
				Y1 = MarginTop,
				X2 = MarginLeft,
				Y2 = Graph.Height - MarginBottom,
				StrokeThickness = 1,
				Stroke = Brushes.Blue,
			});

			//Temperature scale (degrees)
			for (int i = (int)TempMin; i < (int)TempMax; i++)
			{
				Graph.Children.Add(new Line()
				{
					X1 = TimeLineStr - 3,
					Y1 = TempLineStr - ((i - (int)TempMin) * TempLineScale),
					X2 = TimeLineStr,
					Y2 = TempLineStr - ((i - (int)TempMin) * TempLineScale),
					StrokeThickness = 1,
					Stroke = Brushes.Blue,
				});

				if ((i % 5) == 0)
				{
					TextBlock text = new TextBlock()
					{
						Text = i.ToString(),
						FontSize = 9,
					};
					Graph.Children.Add(text);
					Canvas.SetLeft(text, TimeLineStr - 7 - 10);
					Canvas.SetTop(text, TempLineStr - ((i - (int)TempMin) * TempLineScale) - 7);
				}
			}
		}

		private void AddGraphTitle()
		{
			TextBlock text = new TextBlock()
			{ 
				Text = $"Temperature period {PreviousWeathers.Date:yyyy-MM-dd} - " +
					$"{CurrentWeathers.Date:yyyy-MM-dd}",
				FontWeight = FontWeights.Bold
			};
			Graph.Children.Add(text);
			Canvas.SetLeft(text, MarginLeft * 2);
			Canvas.SetTop(text, 5);
		}

		internal void AddTemperatures()
		{
			for (int day = 0; day < 2; day++)
			{
				ObservableCollection<DayWeather> weathers;
				if (day == 0)
				{
					weathers = CurrentWeathers.Weathers;
				}
				else
				{
					weathers = PreviousWeathers.Weathers;
					DateStr = PreviousWeathers.Date;
					DateFin = DateStr.AddDays(1).AddSeconds(-1);
				}

				decimal tempStr = weathers[0].Temperature;
				decimal tempFin;
				TimeSpan timeStr = weathers[0].Time - DateStr;
				TimeSpan timeFin;
				for (int i = 0; i < weathers.Count; i++)
				{
					tempFin = weathers[i].Temperature;
					timeFin = weathers[i].Time - DateStr;

					Line line = new Line()
					{
						X1 = TimeLineStr + timeStr.TotalMinutes * TimeLineScale,
						Y1 = TempLineStr - (double)(tempStr - TempMin) * TempLineScale,
						X2 = TimeLineStr + timeFin.TotalMinutes * TimeLineScale,
						Y2 = TempLineStr - (double)(tempFin - TempMin) * TempLineScale,
						StrokeThickness = 2,
						Stroke = Brushes.Blue,
					};

					if (day == 1)
					{
						line.StrokeThickness = 1;
						line.Stroke = Brushes.Black;
					}
					Graph.Children.Add(line);

					Rectangle area = new Rectangle()
					{
						Width = 4,
						Height = 4,
						Fill = Brushes.Transparent,
						ToolTip = $"time: {weathers[i].Time:dd HH:mm}\ntemp: {weathers[i].Temperature}\u00b0C",
					};
					Graph.Children.Add(area);
					Canvas.SetLeft(area, TimeLineStr + timeStr.TotalMinutes * TimeLineScale - 2);
					Canvas.SetTop(area, TempLineStr - (double)(tempStr - TempMin) * TempLineScale - 2);

					tempStr = tempFin;
					timeStr = timeFin;
				}
			}
		}

		private void AddTemperaturesMilestones()
		{
			//Minimum temperature
			Graph.Children.Add(new Line()
			{
				X1 = TimeLineStr,
				Y1 = TempLineStr - (double)(CurrentWeathers.TemperatureMin - TempMin) * TempLineScale,
				X2 = TimeLineStr + (CurrentWeathers.Date - DateStr).TotalMinutes * TimeLineScale,
				Y2 = TempLineStr - (double)(CurrentWeathers.TemperatureMin - TempMin) * TempLineScale,
				StrokeThickness = 0.5,
				Stroke = Brushes.Green,
			});
			
			//Maximum temperature
			Graph.Children.Add(new Line()
			{
				X1 = TimeLineStr,
				Y1 = TempLineStr - (double)(PreviousWeathers.TemperatureMax - TempMin) * TempLineScale,
				X2 = TimeLineStr + (CurrentWeathers.Date - DateStr).TotalMinutes * TimeLineScale,
				Y2 = TempLineStr - (double)(PreviousWeathers.TemperatureMax - TempMin) * TempLineScale,
				StrokeThickness = 0.5,
				Stroke = Brushes.Green,
			});
			
			//Current temperature
			Graph.Children.Add(new Line()
			{
				X1 = TimeLineStr,
				Y1 = TempLineStr - (double)(CurrentWeathers.Weathers.Last().Temperature - TempMin) * TempLineScale,
				X2 = TimeLineStr + (CurrentWeathers.Date - DateStr).TotalMinutes * TimeLineScale,
				Y2 = TempLineStr - (double)(CurrentWeathers.Weathers.Last().Temperature - TempMin) * TempLineScale,
				StrokeThickness = 0.5,
				Stroke = Brushes.Blue,
			});
			Graph.Children.Add(new Line()
			{
				X1 = TimeLineStr + (CurrentWeathers.Weathers.Last().Time - DateStr.AddDays(1)).TotalMinutes * TimeLineScale,
				Y1 = TempLineStr,
				X2 = TimeLineStr + (CurrentWeathers.Weathers.Last().Time - DateStr.AddDays(1)).TotalMinutes * TimeLineScale,
				Y2 = TempLineFin,
				StrokeThickness = 0.5,
				Stroke = Brushes.Blue,
			});
		}

	}
}
