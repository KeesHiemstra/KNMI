using KMNI.Models;
using KNMI_Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import_Stations
{
  class Program
  {
    static List<Station> Stations = new List<Station>();
    static string LastDate = string.Empty;

    static void Main(string[] args)
    {
      string Downloads = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Downloads";
      string StationsFile = $"{Downloads}\\Stations.txt";

      if (!File.Exists(StationsFile))
      {
        Console.WriteLine($"Can't open {StationsFile}");
      }
      else
      {
        ReadStationsFile(StationsFile);
        SaveStations();
      }

      Console.Write("\nPress any key...");
      Console.ReadKey();
    }
    private static void SaveStations()
    {
      string connection = "Trusted_Connection=True;Data Source=(Local);Database=Weather;MultipleActiveResultSets=true";
      using (var db = new WeatherDbContext(connection))
      {
        foreach (var station in Stations)
        {
          var search = db.Stations
            .Where(x => x.StationCode == station.StationCode)
            .ToList();

          if (search.Count == 0)
          {
            db.Stations.Add(station);
            db.SaveChanges();
          }
        }
      }
    }

    private static void ReadStationsFile(string StationsFile)
    {

      using (StreamReader stream = File.OpenText(StationsFile))
      {
        while (stream.Peek() >= 0)
        {
          string line = stream.ReadLine();
          string[] data = line.Split('\t');
          SaveData(data);
        }
      }

    }

    private static void SaveData(string[] data)
    {

      Station station = new Station();
      station.StationCode = int.Parse(data[0]);
      station.StationName = data[1];
      if (string.IsNullOrEmpty(LastDate))
      {
        LastDate = data[3];
      }
      station.IsActive = data[3] == LastDate;

      Stations.Add(station);

    }
  
  }
}
