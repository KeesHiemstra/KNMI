using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherDemon
{
  public class Logging
  {

    public string LogFile { get; private set; }
    public bool ToConsole { get; set; } = true;

    public Logging(string logFile)
    {
      LogFile = logFile;
    }

    public void Write(string message)
    {
      string _message = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}";
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
