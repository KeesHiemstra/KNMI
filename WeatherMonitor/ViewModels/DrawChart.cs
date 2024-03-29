﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using WeatherDemon.Models;

namespace WeatherMonitor.ViewModels
{
	public class DrawChart
	{

		#region [ Fields ]

		readonly MainViewModel VM;

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

		//PressureLine (Y axis)
		double PressLineStr;
		double PressLineFin;
		double PressLineScale;

		//TimeLine time points
		DateTime DateStr;
		DateTime DateFin;

		decimal TempMin;
		decimal TempMax;
		int PressMin;
		int PressMax;

		#endregion

		#region [ Properties ]

		public Canvas Graph { get; set; } = new Canvas()
		{
			Background = Brushes.LightGray,
			Height = 250,
			Width = 400,
		};

		#endregion

		#region [ Construction ]

		public DrawChart(MainViewModel mainVM)
		{
			VM = mainVM;

			Log.Write("DrawChart: Starting");
			CalculateConstants();
			AddDaylight();
			AddAxis();
			AddGraphTitle();
			AddTemperatures();
			AddPressure();
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

			PressLineStr = Graph.Height - MarginBottom;
			PressLineFin = MarginTop;

			DateStr = VM.CurrentWeathers.Date;
			DateFin = DateStr.AddDays(1).AddSeconds(-1);

			TimeLineScale = (TimeLineFin - TimeLineStr) / (int)(DateFin - DateStr).TotalMinutes;

			TempMin = Math.Floor(VM.PreviousWeathers.TemperatureMin - (decimal)0.25);
			if (VM.CurrentWeathers.TemperatureMin < VM.PreviousWeathers.TemperatureMin)
			{
				TempMin = Math.Floor(VM.CurrentWeathers.TemperatureMin - (decimal)0.25);
			}
			TempMax = Math.Ceiling(VM.PreviousWeathers.TemperatureMax + (decimal)0.25);
			if (VM.CurrentWeathers.TemperatureMax > VM.PreviousWeathers.TemperatureMax)
			{
				TempMax = Math.Ceiling(VM.CurrentWeathers.TemperatureMax + (decimal)0.25);
			}
			TempLineScale = (TempLineStr - TempLineFin) / (double)(TempMax - TempMin);

			PressMin = VM.PreviousWeathers.PressureMin;
			if (VM.CurrentWeathers.PressureMin < VM.PreviousWeathers.PressureMin)
			{
				PressMin = VM.CurrentWeathers.PressureMin;
			}
			PressMin -= 1;
			PressMax = VM.PreviousWeathers.PressureMax;
			if (VM.CurrentWeathers.PressureMax > VM.PreviousWeathers.PressureMax)
			{
				PressMax = VM.CurrentWeathers.PressureMax;
			}
			PressMax += 1;
			PressLineScale = (PressLineStr - PressLineFin) / (double)(PressMax - PressMin);

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
				Width = ((VM.Now.SunsetTime - VM.Now.SunriseTime).TotalMinutes * TimeLineScale),
				Height = TempLineStr - TempLineFin,
				Fill = Brushes.LightCyan,
			};
			Graph.Children.Add(area);
			Canvas.SetLeft(area, TimeLineStr + (VM.Now.SunriseTime - DateStr).TotalMinutes * TimeLineScale);
			Canvas.SetTop(area, MarginTop);
		}

		private void AddAxis()
		{
			AddAxisTime();
			AddAxisTemperature();
			AddAxisPressure();
		}

		private void AddAxisTime()
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

		}

		private void AddAxisTemperature()
		{
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

		private void AddAxisPressure()
		{
			//Pressure axis (Y axis)
			Graph.Children.Add(new Line()
			{
				X1 = Graph.Width,
				Y1 = MarginTop,
				X2 = Graph.Width,
				Y2 = Graph.Height - MarginBottom,
				StrokeThickness = 1,
				Stroke = Brushes.Red,
			});

			//Pressure scale
			for (int i = (int)PressMin; i < (int)PressMax; i++)
			{
				Graph.Children.Add(new Line()
				{
					X1 = Graph.Width - 3,
					Y1 = PressLineStr - ((i - (int)PressMin) * PressLineScale),
					X2 = Graph.Width,
					Y2 = PressLineStr - ((i - (int)PressMin) * PressLineScale),
					StrokeThickness = 1,
					Stroke = Brushes.Red,
				});

				if ((i % 5) == 6)
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
				Text = $"Temperature period {VM.Now.Today.AddDays(-1):yyyy-MM-dd} - " +
					$"{VM.Now.Today:yyyy-MM-dd}",
				FontWeight = FontWeights.Bold
			};
			Graph.Children.Add(text);
			Canvas.SetLeft(text, MarginLeft * 2);
			Canvas.SetTop(text, 5);
		}

		private void AddTemperatures()
		{
			for (int day = 0; day < 2; day++)
			{
				ObservableCollection<DayWeather> weathers;
				if (day == 0)
				{
					weathers = VM.CurrentWeathers.Weathers;
				}
				else
				{
					weathers = VM.PreviousWeathers.Weathers;
					DateStr = VM.PreviousWeathers.Date;
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

		private void AddPressure()
		{
			for (int day = 0; day < 2; day++)
			{
				ObservableCollection<DayWeather> weathers;
				if (day == 0)
				{
					weathers = VM.CurrentWeathers.Weathers;
					DateStr = VM.CurrentWeathers.Date;
				}
				else
				{
					weathers = VM.PreviousWeathers.Weathers;
					DateStr = VM.PreviousWeathers.Date;
					DateFin = DateStr.AddDays(1).AddSeconds(-1);
				}

				decimal pressStr = weathers[0].Pressure;
				decimal pressFin;
				TimeSpan timeStr = weathers[0].Time - DateStr;
				TimeSpan timeFin;
				for (int i = 0; i < weathers.Count; i++)
				{
					pressFin = weathers[i].Pressure;
					timeFin = weathers[i].Time - DateStr;

					Line line = new Line()
					{
						X1 = TimeLineStr + timeStr.TotalMinutes * TimeLineScale,
						Y1 = PressLineStr - (double)(pressStr - PressMin) * PressLineScale,
						X2 = TimeLineStr + timeFin.TotalMinutes * TimeLineScale,
						Y2 = PressLineStr - (double)(pressFin - PressMin) * PressLineScale,
						StrokeThickness = 0.75,
						Stroke = Brushes.Red
					};

					if (day == 1)
					{
						line.StrokeThickness = 0.75;
						line.Stroke = Brushes.DarkRed;
					}
					Graph.Children.Add(line);

					Rectangle area = new Rectangle()
					{
						Width = 4,
						Height = 4,
						Fill = Brushes.Transparent,
						ToolTip = $"time: {weathers[i].Time:dd HH:mm}\npress: {weathers[i].Pressure} hPa",
					};
					Graph.Children.Add(area);
					Canvas.SetLeft(area, TimeLineStr + timeStr.TotalMinutes * TimeLineScale - 2);
					Canvas.SetTop(area, PressLineStr - (double)(pressStr - PressMin) * PressLineScale - 2);

					pressStr = pressFin;
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
				Y1 = TempLineStr - (double)(VM.CurrentWeathers.TemperatureMin - TempMin) * TempLineScale,
				X2 = TimeLineStr + (VM.CurrentWeathers.Date - DateStr).TotalMinutes * TimeLineScale,
				Y2 = TempLineStr - (double)(VM.CurrentWeathers.TemperatureMin - TempMin) * TempLineScale,
				StrokeThickness = 0.5,
				Stroke = Brushes.Green,
			});
			
			//Maximum temperature
			Graph.Children.Add(new Line()
			{
				X1 = TimeLineStr,
				Y1 = TempLineStr - (double)(VM.PreviousWeathers.TemperatureMax - TempMin) * TempLineScale,
				X2 = TimeLineStr + (VM.CurrentWeathers.Date - DateStr).TotalMinutes * TimeLineScale,
				Y2 = TempLineStr - (double)(VM.PreviousWeathers.TemperatureMax - TempMin) * TempLineScale,
				StrokeThickness = 0.5,
				Stroke = Brushes.Green,
			});
			
			//Current temperature
			Graph.Children.Add(new Line()
			{
				X1 = TimeLineStr,
				Y1 = TempLineStr - (double)(VM.CurrentWeathers.Weathers.Last().Temperature - TempMin) * TempLineScale,
				X2 = TimeLineStr + (VM.CurrentWeathers.Date - DateStr).TotalMinutes * TimeLineScale,
				Y2 = TempLineStr - (double)(VM.CurrentWeathers.Weathers.Last().Temperature - TempMin) * TempLineScale,
				StrokeThickness = 0.5,
				Stroke = Brushes.Blue,
			});
			Graph.Children.Add(new Line()
			{
				X1 = TimeLineStr + (VM.CurrentWeathers.Weathers.Last().Time - DateStr.AddDays(1)).TotalMinutes * TimeLineScale,
				Y1 = TempLineStr,
				X2 = TimeLineStr + (VM.CurrentWeathers.Weathers.Last().Time - DateStr.AddDays(1)).TotalMinutes * TimeLineScale,
				Y2 = TempLineFin,
				StrokeThickness = 0.5,
				Stroke = Brushes.Blue,
			});
		}

	}
}
