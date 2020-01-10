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

    public DailyReport Daily { get; set; }

    #endregion

    #region [ Construction ]

    public MainViewModel(MainWindow mainView)
    {

      MainView = mainView;
      Daily = new DailyReport();

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


  }
}
