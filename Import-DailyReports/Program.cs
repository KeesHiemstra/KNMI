using KMNI.Extensions;
using KMNI.Models;
using KNMI_Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Import_DailyReports
{
  class Program
  {
    static public WeatherDbContext Db { get; set; }
    static int RecordCount = 0;
    static int SavedCount = 0;

    static void Main(string[] args)
    {

      string DownloadsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Downloads";
      ReadReportFiles(DownloadsPath);

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

      string connection = "Trusted_Connection=True;Data Source=(Local);Database=Weather;MultipleActiveResultSets=true";
      using (Db = new WeatherDbContext(connection))
      {
        foreach (string file in files)
        {
          ReadReportFile(file);
        }

      }
    }

    /// <summary>
    /// Read daily report.
    /// </summary>
    /// <param name="reportFile"></param>
    private static void ReadReportFile(string reportFile)
    {

      Console.WriteLine($"Processing: {reportFile}");
      RecordCount = 0;
      Console.WriteLine($"Counts: {RecordCount} records, {SavedCount} saved");

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

      DailyReport daily = new DailyReport();

      #region Import
      daily.Stn = int.Parse(data[0]);
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

      RecordCount++;

      var record = Db.Reports
        .AsNoTracking()
        .Where(x => (x.Stn == daily.Stn && x.Date == daily.Date))
        .Count();

      if (record == 0)
      {
        Db.Reports.Add(daily);
        Db.SaveChanges();
        SavedCount++;
      }

      if ((RecordCount % 10000 == 0 && SavedCount == 0) || (RecordCount % 100 == 0 && SavedCount > 0))
      {
        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Counts: {RecordCount} records, {SavedCount} saved ({daily.Date})");
        Thread.Sleep(550);
      }
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

  }
}
