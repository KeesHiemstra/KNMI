using CHi.Extensions;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

using WeatherDemon.Models;

using WeatherMonitor.Models;

namespace WeatherMonitor.ViewModels
{
  public class MainViewModel : INotifyPropertyChanged
  {

    #region [ Fields ]

    MainWindow MainView;

    #endregion

    #region [ Properties ]

    public DailyKNMI Daily { get; set; }
    public VisualTime Now { get; } = new VisualTime();
    public DrawChart DrawChart { get; }

    public DayWeathers CurrentWeathers { get; set; } = new DayWeathers();
    public DayWeathers PreviousWeathers { get; set; } = 
      new DayWeathers(DateTime.Now.Date.AddDays(-1));
    public decimal TotalSunshine { get; set; }

    public DayWeather Today
    {
      get
			{
        return CurrentWeathers.Weathers.LastOrDefault();
			}
    }

    public DayWeather Yesterday
    {
      get
      {
        return PreviousWeathers.Weathers.LastOrDefault();
      }
    }

    #endregion

    #region [ Construction ]

    public MainViewModel(MainWindow mainView)
    {
      MainView = mainView;
      Daily = new DailyKNMI();
      DrawChart = new DrawChart(this);
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
      }
    }
    #endregion

    #endregion


    internal void MainLoaded()
    {
      MainView.GraphStackPanel.Children.Add(DrawChart.Graph);
    }

    private void GetTotalSunshineAsync()
    {
      var range = Daily.GetDailyRange(348, new DateTime(2019, 01, 01), new DateTime(2019, 12, 31));

      foreach (var record in range)
      {
        TotalSunshine += record.SQ.Value;
      }
    }

    private Border GetTemperaturesNumbers()
    {

      DateTime startDate = DateTime.Now.Date.AddDays(-8);
      DateTime endDate = DateTime.Now.Date.AddDays(7);

      var range = Daily.GetDailyRange(348, startDate, endDate);

      StackPanel TemperatureStackPanel = new StackPanel()
      {
        Orientation = Orientation.Vertical
      };
      Border TemperatureBorder = new Border()
      {
        Child = TemperatureStackPanel,
        Margin = new System.Windows.Thickness(5),
        Padding = new System.Windows.Thickness(5),
        BorderThickness = new System.Windows.Thickness(1),
        BorderBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0))
      };
      TextBlock Title = new TextBlock()
      {
        Text = $"Date: {DateTime.Now.Date.ToString("yyyy-MM-dd")}"
      };
      TemperatureStackPanel.Children.Add(Title);

      StackPanel DatesStackPanel = new StackPanel()
      {
        Orientation = Orientation.Horizontal
      };
      foreach (var date in range)
      {
        StackPanel DateStackPanel = new StackPanel()
        {
          Orientation = Orientation.Vertical
        };
        DateStackPanel.Children.Add(new TextBlock() { Text = $"D: {date.Date.Day.ToString("00")} " });
        DateStackPanel.Children.Add(new TextBlock() { Text = $"N: {date.TN.ToString()} " });
        DateStackPanel.Children.Add(new TextBlock() { Text = $"G: {date.TG} " });
        DateStackPanel.Children.Add(new TextBlock() { Text = $"X: {date.TX} " });

        DatesStackPanel.Children.Add(DateStackPanel);
      }
      TemperatureStackPanel.Children.Add(DatesStackPanel);

      return TemperatureBorder;

    }

    private Border GetTemperaturesGraphics(DateTime startDate, int days)
    {

      DateTime endDate = startDate.AddDays(days);

      Border TemperatureBorder = new Border()
      {
        Margin = new System.Windows.Thickness(5),
        Padding = new System.Windows.Thickness(5),
        BorderThickness = new System.Windows.Thickness(1),
        BorderBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))
      };


      return TemperatureBorder;

    }
  }
}
