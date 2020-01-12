using KMNI.Models;
using KNMI_Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

      GetTotalSunshineAsync();

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


    private void GetTotalSunshineAsync()
    {
      var range = Daily.GetDailyRange(348, new DateTime(2019,01,01), new DateTime(2019, 12, 31));

      foreach (var record in range)
      {
        TotalSunshine += record.SQ.Value;
      }
    }

  }
}
