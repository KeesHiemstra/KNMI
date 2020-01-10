using KNMI_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.ViewModels
{
  public class DailyReport
  {

    private const string dbConnection = @"Database=Weather;Data Source=(Local);Trusted_Connection=True;MultipleActiveResultSets=true";

    #region [ Fields ]

    readonly WeatherDbContext Db;
    private DateTime updateDate;

    #endregion

    #region [ Properties ]

    public DateTime UpdateDate { get => updateDate; set => updateDate = value; }

    #endregion

    #region [ Constructor ]

    public DailyReport()
    {

      Db = new WeatherDbContext(dbConnection);
      UpdateDate = GetDailyReportUpdateDate();

    }

    #endregion

    private DateTime GetDailyReportUpdateDate()
    {

      var record = Db.Reports
        .OrderByDescending(x => x.Date)
        .Select(x => x.Date)
        .FirstOrDefault();

      return record;

    }

  }
}
