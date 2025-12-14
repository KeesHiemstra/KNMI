using KNMI.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WeatherMonitor.ViewModels
{
	internal class VisualHistory
	{
		public const string dbConnection = @"Database=Weather;Data Source=(Local);Trusted_Connection=True;MultipleActiveResultSets=true";
		private readonly MainWindow MainView;
		static string[] Arrows = new string[3] { "", "↑ ", "↓ " };

		public static WeatherDbContext Db { get; set; }

		public VisualHistory(MainWindow view)
		{
			MainView = view;
			_ = StartHistory();
		}

		private async Task StartHistory()
		{
			Db = new WeatherDbContext(dbConnection);

			DateTime Date = DateTime.Now.AddDays(-1);

			//History from yesterday till and include to 10 days later
			for (int i = 0; i < 11; i++)
			{
				await CollectHistory(Date.AddDays(i), i);
			}
		}

		private async Task CollectHistory(DateTime dateTime, int count)
		{
			//All data for this day
			List<DailyReport> History = Db.Reports
				.Where(x => x.Stn == 260 &&
					x.Date.Month == dateTime.Month &&
					x.Date.Day == dateTime.Day &&
					x.Date.Year >= dateTime.Year - 31 &&
					x.TX != null &&
					x.TN != null)
				.ToList();

			DailyReport maxTemp = History
				.OrderBy(x => x.TX)
				.ThenBy(x => x.Date)
				.LastOrDefault();
			DailyReport minTemp = History
				.OrderBy(x => x.TN)
				.ThenByDescending(x => x.Date)
				.FirstOrDefault();

			decimal avgMinT10 = History
				.Where(x => x.Date.Year >= (dateTime.Year - 11))
				.Average(x => x.TN)
				.Value;

			string arwMinT10 = ArrowDirection(true, count, avgMinT10);

			decimal avgMaxT10 = History
				.Where(x => x.Date.Year >= (dateTime.Year - 11))
				.Average(x => x.TX)
				.Value;

			string arwMaxT10 = ArrowDirection(false, count, avgMaxT10);

      decimal avgMinT30 = History
				.Where(x => x.Date.Year >= (dateTime.Year - 31))
				.Average(x => x.TN)
				.Value;

			string arwMinT30 = ArrowDirection(true, count, avgMinT30);

      decimal avgMaxT30 = History
				.Where(x => x.Date.Year >= (dateTime.Year - 31))
				.Average(x => x.TX)
				.Value;

			string arwMaxT30 = ArrowDirection(false, count, avgMaxT30);

      BlockHistory(
				maxTemp,
				minTemp,
				avgMinT10,
				arwMinT10,
				avgMaxT10,
				arwMaxT10,
				avgMinT30,
				arwMinT30,
				avgMaxT30,
				arwMaxT30);
		}

		/// <summary>
		/// Fill the BlockHistory to display the data.
		/// </summary>
		/// <param name="maxTemp"></param>
		/// <param name="minTemp"></param>
		/// <param name="avgMinT10"></param>
		/// <param name="avgMaxT10"></param>
		/// <param name="avgMinT30"></param>
		/// <param name="avgMaxT30"></param>

		private void BlockHistory(
			DailyReport maxTemp,
			DailyReport minTemp,
			decimal avgMinT10,
			string arwMinT10,
			decimal avgMaxT10,
			string arwMaxT10,
			decimal avgMinT30,
			string arwMinT30,
			decimal avgMaxT30,
			string arwMaxT30)
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
			};

			panel.Children.Add(new TextBlock()
			{
				Text = $"{arwMinT10}{avgMinT10:0.0} / {arwMaxT10}{avgMaxT10:0.0}",
				FontSize = 10,
				Width = 75,
			});

			panel.Children.Add(new TextBlock()
			{
				Text = $"{arwMinT30}{avgMinT30:0.0} / {arwMaxT30}{avgMaxT30:0.0}",
				FontSize = 10,
				Width = 75,
			});

			panel.Children.Add(new TextBlock()
			{
				Text = " ",
				FontSize = 3,
				Width = 75,
			});

			panel.Children.Add(new TextBlock()
			{
				Text = $"{maxTemp.TX} ({maxTemp.Date.Year})",
				FontSize = 10,
				Width = 75,
			});

			panel.Children.Add(new TextBlock()
			{
				Text = $"{minTemp.TN} ({minTemp.Date.Year})",
				FontSize = 10,
				Width = 75,
			});

			MainView.HistoryStackPanel.Children.Add(border);
		}

    private string ArrowDirection(bool MinTemperatuur, int count, decimal temp)
    {
			if (MainView.Forecasts == null || count > MainView.Forecasts.Count )
      return Arrows[0];

			decimal forecast;
			if (MinTemperatuur)
				forecast = MainView.Forecasts[count].MinTemperature;
			else
				forecast = MainView.Forecasts[count].MaxTemperature;

      if (temp < forecast)
				return Arrows[1];

			if (temp > forecast)
        return Arrows[2];

       return Arrows[0];
    }
  }
}
