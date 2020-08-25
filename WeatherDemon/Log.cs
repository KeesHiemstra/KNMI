using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherDemon
{
  public static class Log
  {

    public static string LogFile { get; private set; }
    public static bool ToConsole { get; set; } = true;

    public static void File(string logFile)
    {
      LogFile = logFile;
    }

    public static void Write(string message)
    {
      string _message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";
      if (ToConsole)
      {
        Console.WriteLine(_message);
      }

      using (StreamWriter stream = new StreamWriter(LogFile, true))
      {
        stream.WriteLine(_message);
      }
    }
  }
}
