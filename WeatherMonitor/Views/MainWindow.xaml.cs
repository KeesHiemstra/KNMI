using CHi.Extensions;

using System.Windows;

using WeatherMonitor.Models;
using WeatherMonitor.ViewModels;

namespace WeatherMonitor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
  {

    public MainViewModel MainVM { get; set; }

    public MainWindow()
    {

      Log.Write($"Application {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} started");

      MainVM = new MainViewModel(this);

      InitializeComponent();

      DataContext = MainVM;

    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      MainVM.MainLoaded();
    }
  }
}
