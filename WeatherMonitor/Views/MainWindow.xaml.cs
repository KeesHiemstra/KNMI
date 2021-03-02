using CHi.Extensions;

using System.Windows;
using System.Windows.Input;

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

    #region Menu MainCommands

    #region Exit command
    private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void ExitCommand_Execute(object sender, ExecutedRoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }
    #endregion

    #endregion


  }
}
