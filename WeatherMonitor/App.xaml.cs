using CHi.Extensions;
using System.Windows;

using WeatherMonitor.ViewModels;

namespace WeatherMonitor
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      if (!ServiceExtensions.IsStarted("MSSQLServer", true))
      {
        Log.Write("Microsoft SQL Server is not started");
        Application.Current.Shutdown();
      }
    }
  }
}
