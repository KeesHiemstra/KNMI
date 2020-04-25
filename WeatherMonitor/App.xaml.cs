using CHi.Extensions;
using System.Windows;

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
        Application.Current.Shutdown();
      }
    }
  }
}
