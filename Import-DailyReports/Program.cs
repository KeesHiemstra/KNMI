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
      string date = data[1];
      DailyReport daily = new DailyReport();

      #region Import
      if (!string.IsNullOrWhiteSpace(data[2])) { daily.DDVec = int.Parse(data[2]); }
      if (!string.IsNullOrWhiteSpace(data[3])) { daily.FHVec = int.Parse(data[3]); }
      if (!string.IsNullOrWhiteSpace(data[4])) { daily.FG = int.Parse(data[4]); }
      if (!string.IsNullOrWhiteSpace(data[5])) { daily.FHX = int.Parse(data[5]); }
      if (!string.IsNullOrWhiteSpace(data[6])) { daily.FHXH = int.Parse(data[6]); }
      if (!string.IsNullOrWhiteSpace(data[7])) { daily.FHN = int.Parse(data[7]); }
      if (!string.IsNullOrWhiteSpace(data[8])) { daily.FHNH = int.Parse(data[8]); }
      if (!string.IsNullOrWhiteSpace(data[9])) { daily.FXX = int.Parse(data[9]); }
      if (!string.IsNullOrWhiteSpace(data[10])) { daily.FXXH = int.Parse(data[10]); }
      if (!string.IsNullOrWhiteSpace(data[11])) { daily.TG = int.Parse(data[11]); }
      if (!string.IsNullOrWhiteSpace(data[12])) { daily.TN = int.Parse(data[12]); }
      if (!string.IsNullOrWhiteSpace(data[13])) { daily.TNH = int.Parse(data[13]); }
      if (!string.IsNullOrWhiteSpace(data[14])) { daily.TX = int.Parse(data[14]); }
      if (!string.IsNullOrWhiteSpace(data[15])) { daily.TXH = int.Parse(data[15]); }
      if (!string.IsNullOrWhiteSpace(data[16])) { daily.T10N = int.Parse(data[16]); }
      if (!string.IsNullOrWhiteSpace(data[17])) { daily.T10NH = int.Parse(data[17]); }
      if (!string.IsNullOrWhiteSpace(data[18])) { daily.SQ = int.Parse(data[18]); }
      if (!string.IsNullOrWhiteSpace(data[19])) { daily.SP = int.Parse(data[19]); }
      if (!string.IsNullOrWhiteSpace(data[20])) { daily.Q = int.Parse(data[20]); }
      if (!string.IsNullOrWhiteSpace(data[21])) { daily.DR = int.Parse(data[21]); }
      if (!string.IsNullOrWhiteSpace(data[22])) { daily.RH = int.Parse(data[22]); }
      if (!string.IsNullOrWhiteSpace(data[23])) { daily.RHX = int.Parse(data[23]); }
      if (!string.IsNullOrWhiteSpace(data[24])) { daily.RHXH = int.Parse(data[24]); }
      if (!string.IsNullOrWhiteSpace(data[25])) { daily.PG = int.Parse(data[25]); }
      if (!string.IsNullOrWhiteSpace(data[26])) { daily.PX = int.Parse(data[26]); }
      if (!string.IsNullOrWhiteSpace(data[27])) { daily.PXH = int.Parse(data[27]); }
      if (!string.IsNullOrWhiteSpace(data[28])) { daily.PN = int.Parse(data[28]); }
      if (!string.IsNullOrWhiteSpace(data[29])) { daily.PNH = int.Parse(data[29]); }
      if (!string.IsNullOrWhiteSpace(data[30])) { daily.VVN = int.Parse(data[30]); }
      if (!string.IsNullOrWhiteSpace(data[31])) { daily.VVNH = int.Parse(data[31]); }
      if (!string.IsNullOrWhiteSpace(data[32])) { daily.VVX = int.Parse(data[32]); }
      if (!string.IsNullOrWhiteSpace(data[33])) { daily.VVXH = int.Parse(data[33]); }
      if (!string.IsNullOrWhiteSpace(data[34])) { daily.NG = int.Parse(data[34]); }
      if (!string.IsNullOrWhiteSpace(data[35])) { daily.UG = int.Parse(data[35]); }
      if (!string.IsNullOrWhiteSpace(data[36])) { daily.UX = int.Parse(data[36]); }
      if (!string.IsNullOrWhiteSpace(data[37])) { daily.UXH = int.Parse(data[37]); }
      if (!string.IsNullOrWhiteSpace(data[38])) { daily.UN = int.Parse(data[38]); }
      if (!string.IsNullOrWhiteSpace(data[39])) { daily.UNH = int.Parse(data[39]); }
      if (!string.IsNullOrWhiteSpace(data[40])) { daily.EV24 = int.Parse(data[40]); }
      #endregion

      if (!Reports.ContainsKey(Stn))
      {
        Reports.Add(Stn, new List<DailyReport>());
      }

      Reports[Stn].Add(daily);

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
