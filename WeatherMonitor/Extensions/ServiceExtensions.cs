using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CHi.Extensions
{
  /// <summary>
  /// Example: Console.WriteLine($"The SQL Server is running: {"MSSQLSERVER".IsStarted()}");
  /// Example: Console.WriteLine(ServiceExtensions.IsStarted("MSSQLServer", true, true));
  /// It must be run under administrative rights.
  /// 
  /// Version 1.0.0.1 (2020-04-25, Kees Hiemstra)
  /// </summary>
  public static class ServiceExtensions
  {
    /// <summary>
    /// Is the service 'name' started.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="visualIfNotStarted"></param>
    /// <param name="startService"></param>
    /// <returns></returns>
    public static bool IsStarted
      (
        this string name,
        bool visualIfNotStarted = false,
        bool startService = false
      )
    {
      bool result = false;

      ServiceController serviceController = new ServiceController(name);
      try
      {
        if (serviceController.Status == ServiceControllerStatus.Running)
        {
          result = true;
        }
        else
        {
          if (visualIfNotStarted)
          {
            if (!startService)
            {
              MessageBox.Show($"The service '{name}' is not running",
                $"The service: {name}",
                MessageBoxButton.OK,
                MessageBoxImage.Error
                );
            }
            else
            {
              if (MessageBox.Show($"The service '{name}' is not running",
                $"The service: {name}",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes
                ) == MessageBoxResult.Yes)
              {
                serviceController.Refresh();
                try
                {
                  serviceController.Start();
                }
                catch (Exception ex)
                {
                  MessageBox.Show($"{ex.Message}",
                    $"The service: {name}",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                  return false;
                }
                int wait = 0;
                while (wait < 20 && serviceController.Status != ServiceControllerStatus.Running)
                {
                  Thread.Sleep(500);
                  serviceController.Refresh();
                  wait++;
                }
                if (serviceController.Status == ServiceControllerStatus.Running)
                {
                  result = true;
                }
                else
                {
                  MessageBox.Show($"The service '{name}' is still not running",
                    $"The service: {name}",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                }
              }
            }
          }
        }
      }
      catch
      {
        if (visualIfNotStarted)
        {
          MessageBox.Show($"The service '{name}' doesn't exists",
            $"The service: {name}",
            MessageBoxButton.OK,
            MessageBoxImage.Error
            );
        }
      }

      return result;
    }

    /// <summary>
    /// Does exist the service 'name'.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool DoesExist(this string name)
    {
      bool result = false;

      ServiceController serviceController = new ServiceController(name);
      try
      {
        if (!string.IsNullOrEmpty(serviceController.ServiceName))
        {
          result = true;
        }
      }
      catch { }

      return result;
    }
  }
}
