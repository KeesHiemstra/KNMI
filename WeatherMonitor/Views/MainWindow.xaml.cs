using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
