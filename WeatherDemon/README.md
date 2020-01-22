# WeatherDemon

This demon collects the actual weather information every 
20 minutes for a maximal 24 hour in a json file.

The json can be used the progress of the weather.

The information uses

- Date [*yyyy-MM-dd HH:mm*]
- Temperature [*Celsius*]
- Pressure [*hPa*]
- Humidity [*%*]
- Visibility [*meter*]
- WindSpeed [*km/u*]
- WindDirection [*degrees*]

## Source

The source is [OpenWeatherMap](https://openweathermap.org/).

OpenWeather uses personal ID and I stored it in 
*DemonOpenWeather.json* in a private location *%OneDrive%\\Etc*.

There are no costs for a personal ID, but the is a usage
limit.

## Example for DemonOpenWeather.json
```javascript
{
  "Latitude": <Latitude>,
  "Longitude": <Longitude>,
  "AppID": "<Application ID>",
  "Language": "nl"
}
```
