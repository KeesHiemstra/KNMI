using CHi.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WeatherMonitor.Models;
using WeatherMonitor.ViewModels;

namespace SendChart
{
	class Program
	{
		public static DateTime CurrentDate;
		public static DateTime PreviousDate;
		public static DayWeathers CurrentWeathers;
		public static DayWeathers PreviousWeathers;

		[STAThread]
		static void Main(string[] args)
		{
			CurrentDate = new DateTime(2020, 10, 03);
			PreviousDate = new DateTime(2020, 10, 02);

			CurrentWeathers = new DayWeathers(CurrentDate);
			PreviousWeathers = new DayWeathers(PreviousDate);

			DrawChart chart = new DrawChart(CurrentWeathers, PreviousWeathers, CurrentDate);
			RenderTargetBitmap bitmap =
				new RenderTargetBitmap(400, 250, 96d, 96d, PixelFormats.Default);
			bitmap.Render(chart.Graph);

			BitmapEncoder pngEncoder = new PngBitmapEncoder();
			pngEncoder.Frames.Add(BitmapFrame.Create(bitmap));

			//try
			//{
			//	MemoryStream ms = new MemoryStream();

			//	pngEncoder.Save(ms);
			//	ms.Close();

			//	File.WriteAllBytes($"DayWeathers_{CurrentDate:yyyy-MM-dd}.png", ms.ToArray());
			//}
			//catch (Exception err)
			//{
			//	MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			//}

			Console.Write("\nPress any key...");
			Console.ReadKey();
		}

	}
}
