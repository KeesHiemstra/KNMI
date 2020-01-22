using System;
using System.Threading;

namespace WeatherDemon
{
  public class Program
  {
    public static Logging Log = new Logging("WeatherDemon.log");

    public static void Main()
    {
      Program program = new Program();
      Log.Write("WeatherDemon is started");
#if DEBUG
      program.StartTimer(2 * 60 * 1000);
#else
      program.StartTimer(20 * 60 * 1000);
#endif

      Console.WriteLine("Press enter to end the demon...");
      Console.ReadLine();
      Log.Write("WeatherDemon is properly ended");
    }

    public void StartTimer(int repeatTime)
    {
      //Set and start the timer.
      Timer timer = new Timer(new TimerCallback(TimerEvent));
      timer.Change(0, repeatTime);
    }

    private void TimerEvent(object state)
    {
      Timer timer = (Timer)state;
      Log.Write("Start TimeEvent");
      _ = new Demon();
    }


  }
}
