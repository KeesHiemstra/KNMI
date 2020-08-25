using System;
using System.IO;

namespace WeatherMonitor.ViewModels
{
  public static class Log
  {

    #region [ Fields ]

    public static string LoggingFile;

    #endregion

    public static string SetLoggingFile(string loggingFile)
    {

      LoggingFile = loggingFile;
      return loggingFile;

    }

    public static void Write(string message)
    {
      if (string.IsNullOrEmpty(LoggingFile))
      {
        string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        string lf = SetLoggingFile($".\\{name}.log");

        if (File.Exists(LoggingFile))
        {
          using StreamWriter stream = new StreamWriter(LoggingFile, true);
          stream.WriteLine();
        }
        Write($"Logging '{name}' started using '{lf}'");
      }

      string Message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";
      using (StreamWriter stream = new StreamWriter(LoggingFile, true))
      {
        stream.WriteLine(Message);
      }
    }
  }
}
