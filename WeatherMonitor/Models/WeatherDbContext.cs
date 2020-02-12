using System.Data.Entity;

namespace KNMI.Models
{
  public class WeatherDbContext : DbContext
  {
    public WeatherDbContext(string dbConnection) : base(dbConnection) { }

    public DbSet<DailyReport> Reports { get; set; }

    public DbSet<Station> Stations { get; set; }

    internal static void SeedData(WeatherDbContext context)
    {
      context.Database.CreateIfNotExists();
    }
  }
}
