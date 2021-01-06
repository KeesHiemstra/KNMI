using CHi.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherDemon.Models;

namespace WeatherDemon
{
  public class Demon : IDisposable
  {
    private readonly string DemonAppJsonFile = 
      "%OneDrive%\\Etc\\DemonOpenWeather.json".TranslatePath();
#if DEBUG
    private readonly string DemonDayWeatherJsonFile = 
      "%OneDrive%\\Tmp\\DailyWeather\\DayWeather.json".TranslatePath();
    private readonly string DemonBackupWeatherJsonFile =
      $"%OneDrive%\\Tmp\\DailyWeather\\DayWeather_{DateTime.Now.Date:yyyy-MM-dd}.json".TranslatePath();
#else
    private readonly string DemonDayWeatherJsonFile = 
      "%OneDrive%\\Data\\DailyWeather\\DayWeather.json".TranslatePath();
    private readonly string DemonBackupWeatherJsonFile = 
      $"%OneDrive%\\Data\\DailyWeather\\DayWeather_{DateTime.Now.Date.ToString("yyyy-MM-dd")}.json".TranslatePath();
#endif
    private DemonApp DemonApp = new DemonApp();
    
    private DayWeather DayWeather { get; set; } = new DayWeather();
    public List<DayWeather> DayWeathers { get; set; } = new List<DayWeather>();

    public Demon()
    {

      Run();
      Dispose();
      
    }

    private async void Run()
    {

      LoadDemonOpenWeatherJson();
      LoadDayWeather();

			try
			{
        await GetDayWeather();
      }
      catch (Exception ex)
			{
        Log.Write($"Error GetDayWeather(): {ex.Message}");
			}
      BackupDayWeather();

    }

    private void LoadDemonOpenWeatherJson()
    {
      
      if (File.Exists(DemonAppJsonFile))
      {
        using (StreamReader stream = File.OpenText(DemonAppJsonFile))
        {
          string json = stream.ReadToEnd();
          DemonApp = JsonConvert.DeserializeObject<DemonApp>(json);
        }
      }

    }

    private void LoadDayWeather()
    {

      if (File.Exists(DemonDayWeatherJsonFile))
      {
        using (StreamReader stream = File.OpenText(DemonDayWeatherJsonFile))
        {
          string json = stream.ReadToEnd();
          DayWeathers = JsonConvert.DeserializeObject<List<DayWeather>>(json);
        }
      }

    }

    private async Task GetDayWeather()
    {

      string url = $"http://api.openweathermap.org/data/2.5/weather?" +
        $"lat={DemonApp.Latitude}&" +
        $"lon={DemonApp.Longitude}&" +
        $"appid={DemonApp.AppID}&" +
        $"lang={DemonApp.Language}";

      HttpClient http = new HttpClient();
      HttpResponseMessage httpRespond =
        await http.GetAsync(url);
      string httpResult = await httpRespond.Content.ReadAsStringAsync();

      OpenWeather openWeather = JsonConvert.DeserializeObject<OpenWeather>(httpResult);
      Log.Write($"Longitude: {openWeather.coord.lon}, Latitude: {openWeather.coord.lat}");
			if (httpRespond.StatusCode != System.Net.HttpStatusCode.OK)
			{
        Log.Write($"HttpRespond: {httpRespond}");
      }
      ConvertDayWeather(openWeather);

      SaveDayWeather();

    }

    private void ConvertDayWeather(OpenWeather openWeather)
    {
      DayWeather.DemonTime = DateTime.Now;
      DayWeather.Time = openWeather.dt.ConvertUnixTimeToDate();
      DayWeather.Temperature = openWeather.main.temp.ToCelsius();
      DayWeather.Pressure = openWeather.main.pressure;
      DayWeather.Humidity = openWeather.main.humidity;
      DayWeather.Visibility = openWeather.visibility;
      DayWeather.WindSpeed = openWeather.wind.speed.ToKmPerHour();
      DayWeather.WindDirection = openWeather.wind.deg;
      DayWeather.Covering = openWeather.clouds.all;
      DayWeather.Condition = openWeather.weather[0].id;

    }

    private void SaveDayWeather()
    {

      DayWeathers.Add(DayWeather);
      string json = JsonConvert.SerializeObject(DayWeathers.
        Where(x => x.Time > DateTime.Now.AddMinutes(-24 * 60)), Formatting.Indented);
      using (StreamWriter stream = new StreamWriter(DemonDayWeatherJsonFile))
      {
        stream.Write(json);
      }

    }

    public void Dispose() { }

    private void BackupDayWeather()
    {

      File.Copy(DemonDayWeatherJsonFile, DemonBackupWeatherJsonFile, true);

    }

  }

  public class DemonApp
  {
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string AppID { get; set; }
    public string Language { get; set; }
  }
}
