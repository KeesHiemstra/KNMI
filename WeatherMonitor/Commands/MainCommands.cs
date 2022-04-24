using System.Windows.Input;

namespace WeatherMonitor.Commands
{
	public static class MainCommands
	{
		public static readonly RoutedUICommand Exit = new RoutedUICommand
			(
				"E_xit",
				"Exit",
				typeof(MainCommands),
				new InputGestureCollection()
				{
					new KeyGesture(Key.F4, ModifierKeys.Alt),
					new KeyGesture(Key.W, ModifierKeys.Control),
				}
			);

		public static readonly RoutedUICommand Restart = new RoutedUICommand
			(
				"_Restart",
				"Restart",
				typeof(MainCommands),
				new InputGestureCollection()
				{
					new KeyGesture(Key.R, ModifierKeys.Control),
				}
			);

	}
}
