using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WeatherMonitor.Models;

namespace WeatherMonitor.ViewModels
{
	/// <summary>
	/// The collection of the forecasts from yesterday and forward.
	/// </summary>
	public class VisualForecasts
	{
		private readonly string ForecastsJsonPath = @"\\Rommeldijk\Data\Forecasts.json";
		private readonly MainWindow MainView;

		public List<Forecast> AllForecasts { get; set; }
		//Only current data
		public List<Forecast> Forecasts { get; set; } = new List<Forecast>();

		public VisualForecasts(MainWindow view)
		{
			MainView = view;
			_ = StartCollecting();
		}

		private async Task StartCollecting()
		{
			await CollectForecasts();
			foreach (Forecast item in Forecasts)
			{
				MainView.ForecastsStackPanel.Children.Add(await BlockForecast(item));
			}
		}

		private async Task CollectForecasts()
		{
			Forecasts.Clear();
			if (!File.Exists(ForecastsJsonPath)) { return; }

			//Raw data
			AllForecasts = new List<Forecast>();
			using StreamReader stream = File.OpenText(ForecastsJsonPath);
			string json = stream.ReadToEnd();
			AllForecasts = JsonConvert.DeserializeObject<List<Forecast>>(json);

			//The forecast of yesterday
			DateTime day = DateTime.Now.Date.AddDays(-1);
			Forecast forecast = AllForecasts
				.OrderBy(x => x.MeasureDate)
				.ThenBy(x => x.ForecastDate)
				.Where(x => x.MeasureDate >= day &&
					x.MeasureDate < day.AddDays(1) &&
					x.ForecastDate == day)
				.LastOrDefault();
			if (forecast != null)
			{
				Forecasts.Add(forecast);
			}

			//The forecasts from today
			day = DateTime.Now.Date;
			foreach (Forecast item in AllForecasts
				.OrderBy(x => x.MeasureDate)
				.ThenBy(x => x.ForecastDate)
				.Where(x => x.MeasureDate == day))
			{
				Forecasts.Add(item);
			}
		}

		private async Task<Border> BlockForecast(Forecast forecast)
		{
			StackPanel panel = new StackPanel()
			{
				Orientation = Orientation.Vertical,
			};

			Border border = new Border()
			{
				BorderBrush = Brushes.LightGray,
				BorderThickness = new Thickness(0.5),
				Child = panel,
				Padding = new Thickness(1),
				ToolTip = new ToolTip()
				{
					StaysOpen = true,
					Content = await ToolTipForecast(forecast),
				},
			};

			TextBlock output = new TextBlock()
			{
				Text = forecast.ForecastDate.ToString("ddd d"),
				FontSize = 13,
				FontWeight = FontWeights.Bold,
			};
			if (forecast.ForecastDate < DateTime.Now.Date)
			{
				output.Foreground = Brushes.DarkGray;
			}
			panel.Children.Add(output);

			panel.Children.Add(new TextBlock()
			{
				Text = forecast.WeatherType,
				FontSize = 10,
				Width = 75,
				TextWrapping = TextWrapping.WrapWithOverflow,
			});
			panel.Children.Add(new TextBlock()
			{
				Text = forecast.MaxTemperature.ToString(),
				Width = 75,
			});
			panel.Children.Add(new TextBlock()
			{
				Text = forecast.MinTemperature.ToString(),
				Width = 75,
			});

			return border;
		}

		private async Task<StackPanel> ToolTipForecast(Forecast forecast)
		{
			StackPanel panel = new StackPanel()
			{
				Orientation = Orientation.Vertical,
			};

			var list = AllForecasts
				.Where(x => x.ForecastDate == forecast.ForecastDate)
				.OrderByDescending(x => x.MeasureDate);

			foreach (var item in list)
			{
				panel.Children.Add(await ToolTipForecastLine(item));
			}

			return panel;
		}

		private async Task<StackPanel> ToolTipForecastLine(Forecast forecast)
		{
			StackPanel panel = new StackPanel()
			{
				Orientation = Orientation.Horizontal,
			};

			_ = panel.Children.Add(new TextBlock()
			{
				Text = forecast.WeatherType,
				Width = 75,
				TextWrapping = TextWrapping.WrapWithOverflow,
			});

			_ = panel.Children.Add(new TextBlock()
			{
				Text = forecast.MaxTemperature.ToString(),
				Padding = new Thickness(0, 0, 3, 0),
				Width = 20,
			});

			_ = panel.Children.Add(new TextBlock()
			{
				Text = forecast.MinTemperature.ToString(),
				Width = 20,
			});

			return panel;
		}

	}
}
