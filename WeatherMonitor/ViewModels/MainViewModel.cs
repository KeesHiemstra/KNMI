using KMNI.Extensions;
using KMNI.Models;
using KNMI_Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WeatherMonitor.ViewModels
{
  public class MainViewModel : INotifyPropertyChanged
  {

    #region [ Fields ]

    MainWindow MainView;

    #endregion

    #region [ Properties ]

    public DailyKNMI Daily { get; set; }

    public decimal TotalSunshine { get; set; }

    #endregion

    #region [ Construction ]

    public MainViewModel(MainWindow mainView)
    {

      MainView = mainView;
      Daily = new DailyKNMI();

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
      //GetTotalSunshineAsync();
      //MainView.ResultStackPanel.Children.Add(GetTemperaturesNumbers());
      //MainView.ResultStackPanel.Children.Add(GetTemperaturesGraphics(DateTime.Now.Date, 14));

      //.Select(x => (X.Date, x.TN, x.TG, x.TX)) resulted as Item1, Item2, ...
      string json = JsonConvert.SerializeObject(Daily.GetDailyRange(356, DateTime.Now.Date.AddDays(-29), DateTime.Now.Date.AddDays(14))
        .Select(x => new 
        { 
          Date = x.Date, 
          TN = x.TN, 
          TG = x.TG, 
          TX = x.TX 
        })
        .ToList(), Formatting.Indented);
      using (StreamWriter stream = new StreamWriter("%OneDrive%\\Tmp\\Data.json".TranslatePath()))
      {
        stream.Write(json);
      }

    }

    private void GetTotalSunshineAsync()
    {
      var range = Daily.GetDailyRange(348, new DateTime(2019,01,01), new DateTime(2019, 12, 31));

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
        BorderBrush = new SolidColorBrush(Color.FromArgb(128,255,0,0))
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
