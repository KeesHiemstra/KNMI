using KMNI.Extensions;
using KMNI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import_DailyReports
{
  class Program
  {
    static readonly public DateTime FirstDate = DateTime.Parse("1968-01-01");

    static Dictionary<int, List<DailyReport>> Reports { get; set; } = new Dictionary<int, List<DailyReport>>();

    static void Main(string[] args)
    {

      string DownloadsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Downloads";
      ReadReportFiles(DownloadsPath);

      SaveReports();

      Console.Write("\nPress any key...");
      Console.ReadKey();

    }

    /// <summary>
    /// Find daily reports.
    /// </summary>
    /// <param name="downloads"></param>
    private static void ReadReportFiles(string downloads)
    {

      IEnumerable<string> files = Directory.EnumerateFiles(downloads, "etmgeg_*.txt", SearchOption.TopDirectoryOnly);

      foreach (string file in files)
      {
        ReadReportFile(file);
      }

    }

    /// <summary>
    /// Read daily report.
    /// </summary>
    /// <param name="reportFile"></param>
    private static void ReadReportFile(string reportFile)
    {

      // Strip the header
      using (StreamReader stream = File.OpenText(reportFile))
      {
        bool dataIsStarted = false;
        string line;
        while (stream.Peek() >= 0)
        {
          line = stream.ReadLine();

          if (dataIsStarted)
          {
            if (!string.IsNullOrWhiteSpace(line))
            {
              ProcessLine(line);
            }
          }
          else
          {
            dataIsStarted = (!string.IsNullOrWhiteSpace(line) && line[0] == '#');
          }
        }
      }

    }

    /// <summary>
    /// Process data from daily report.
    /// </summary>
    /// <param name="line"></param>
    private static void ProcessLine(string line)
    {

      string[] data = line.Split(',');

      int Stn = int.Parse(data[0]);
      if (TranslateDate(data[1]) < FirstDate)
      {
        return;
      }
      DailyReport daily = new DailyReport();

      #region Import
      daily.Date = TranslateDate(data[1]);
      daily.DDVec = TranslateInt(data[2]);
      daily.FHVec = TranslateDouble(data[3]);
      daily.FG = TranslateDouble(data[4]); 
      daily.FHX = TranslateDouble(data[5]); 
      daily.FHXH = TranslateInt(data[6]); 
      daily.FHN = TranslateDouble(data[7]); 
      daily.FHNH = TranslateInt(data[8]);
      daily.FXX = TranslateDouble(data[9]); 
      daily.FXXH = TranslateInt(data[10]);
      daily.TG = TranslateDouble(data[11]); 
      daily.TN = TranslateDouble(data[12]); 
      daily.TNH = TranslateInt(data[13]);
      daily.TX = TranslateDouble(data[14]); 
      daily.TXH = TranslateInt(data[15]);
      daily.T10N = TranslateDouble(data[16]);
      daily.T10NH = TranslateInt(data[17]);
      daily.SQ = TranslateDouble(data[18]);
      daily.SP = TranslateInt(data[19]);
      daily.Q = TranslateInt(data[20]); 
      daily.DR = TranslateDouble(data[21]);
      daily.RH = TranslateDouble(data[22]);
      daily.RHX = TranslateDouble(data[23]);
      daily.RHXH = TranslateInt(data[24]);
      daily.PG = TranslateDouble(data[25]); 
      daily.PX = TranslateDouble(data[26]); 
      daily.PXH = TranslateInt(data[27]);
      daily.PN = TranslateDouble(data[28]); 
      daily.PNH = TranslateInt(data[29]);
      daily.VVN = TranslateInt(data[30]);
      daily.VVNH = TranslateInt(data[31]);
      daily.VVX = TranslateInt(data[32]); 
      daily.VVXH = TranslateInt(data[33]);
      daily.NG = TranslateInt(data[34]); 
      daily.UG = TranslateInt(data[35]); 
      daily.UX = TranslateInt(data[36]); 
      daily.UXH = TranslateInt(data[37]);
      daily.UN = TranslateInt(data[38]); 
      daily.UNH = TranslateInt(data[39]);
      daily.EV24 = TranslateDouble(data[40]);
      #endregion

      if (!Reports.ContainsKey(Stn))
      {
        Reports.Add(Stn, new List<DailyReport>());
      }

      Reports[Stn].Add(daily);

    }

    private static DateTime TranslateDate(string str)
    {
      // Format: YYYYMMDD => yyyy-MM-dd
      str = str.Insert(6, "-").Insert(4, "-");
      return DateTime.Parse(str);
    }

    private static int? TranslateInt(string str)
    {
      int? data = null;

      if (!string.IsNullOrWhiteSpace(str))
      {
        data = int.Parse(str);
      }

      return data;
    }

    private static double? TranslateDouble(string str)
    {
      double? data = null;

      if (!string.IsNullOrWhiteSpace(str))
      {
        data = double.Parse(str) / 10;
      }

      return data;
    }

    /// <summary>
    /// Save process data in json file.
    /// </summary>
    private static void SaveReports()
    {
      foreach (var report in Reports)
      {
        string JsonPath = $"%OneDrive%\\Data\\KNMI\\Daily_{report.Key.ToString()}.json";
        string json = JsonConvert.SerializeObject(report.Value, Formatting.Indented);
        using (StreamWriter stream = new StreamWriter(JsonPath.TranslatePath()))
        {
          stream.Write(json);
        }

      }
    }

  }
}
