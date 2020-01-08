using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherMonitor.Models;

namespace WeatherMonitor.Data
{
  public class OpenWeatherService
  {
    public async Task<OpenWeather> GetOpenWeatherAsync()
    {
			//Get JSon string from the web
			HttpClient http = new HttpClient();
			HttpResponseMessage httpRespond =
				await http.GetAsync($"http://api.openweathermap.org/data/2.5/weather?lat=51.913580&lon=4.999771&appid=8b6fbc4fab942058a2e1967b913460a4&lang=nl");
			string httpResult = await httpRespond.Content.ReadAsStringAsync();

			OpenWeather openWeather = JsonConvert.DeserializeObject<OpenWeather>(httpResult);

			return openWeather;
		}
	}
}
