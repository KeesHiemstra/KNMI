# WeatherDemon

This demon collects the actual weather information every 
10 minutes for a maximal 24 hour in a json file.

The json can be used the progress of the weather.

The information uses

- DemonTime [*yyyy-MM-dd HH:mm*]
- Time [*yyyy-MM-dd HH:mm*]
- Temperature [*Celsius* / *�C*]
- Pressure [*hPa*]
- Humidity [*%*]
- Visibility [*m*]
- WindSpeed [*km/h*]
- WindDirection [*degrees*]
- Covering [*%*]
- Condition [*id*]

### Time

The time is received from the OpenWeather object. It contains sometime a cashed information.

### DemonTime

The DemonTime is the time when the Demon runs (System time).

## Source

The source is [OpenWeatherMap](https://openweathermap.org/).

OpenWeather uses personal ID and I stored it in 
*DemonOpenWeather.json* in a private location *%OneDrive%\\Etc*.

There are no costs for a personal ID, but the is a usage
limit.

### Weather condition codes

#### Group 200-299 / Thunderstorm

Id | Main | Description
---|---|---
200 | Thunderstorm | thunderstorm with light rain
201 | Thunderstorm | thunderstorm with rain
202 | Thunderstorm | thunderstorm with heavy rain
210 | Thunderstorm | light thunderstorm
211 | Thunderstorm | thunderstorm
212 | Thunderstorm | heavy thunderstorm
221 | Thunderstorm | ragged thunderstorm
230 | Thunderstorm | thunderstorm with light drizzle
231 | Thunderstorm | thunderstorm with drizzle
232 | Thunderstorm | thunderstorm with heavy drizzle

#### Group 300-399 / Drizzle

Id | Main | Description
---|---|---
300 | Drizzle | light intensity drizzle
301 | Drizzle | drizzle
302 | Drizzle | heavy intensity drizzle
310 | Drizzle | light intensity drizzle rain
311 | Drizzle | drizzle rain
312 | Drizzle | heavy intensity drizzle rain
313 | Drizzle | shower rain and drizzle
314 | Drizzle | heavy shower rain and drizzle
321 | Drizzle | shower drizzle

#### Group 500-599 / Rain

Id | Main | Description
---|---|---
500 | Rain | light rain
501 | Rain | moderate rain
502 | Rain | heavy intensity rain
503 | Rain | very heavy rain
504 | Rain | extreme rain
511 | Rain | freezing rain
520 | Rain | light intensity shower rain
521 | Rain | shower rain
522 | Rain | heavy intensity shower rain
531 | Rain | ragged shower rain

#### Group 600-699 / Snow

Id | Main | Description
---|---|---
600 | Snow | light snow
601 | Snow | Snow
602 | Snow | Heavy snow
611 | Snow | Sleet
612 | Snow | Light shower sleet
613 | Snow | Shower sleet
615 | Snow | Light rain and snow
616 | Snow | Rain and snow
620 | Snow | Light shower snow
621 | Snow | Shower snow
622 | Snow | Heavy shower snow

#### Group 700-799 / Atmosphere

Id | Main | Description
---|---|---
701 | Mist | mist
711 | Smoke | Smoke
721 | Haze | Haze
731 | Dust | sand/ dust whirls
741 | Fog | fog
751 | Sand | sand
761 | Dust | dust
762 | Ash | volcanic ash
771 | Squall | squalls
781 | Tornado | tornado

#### Group 800-899 / Clear or clouds

Id | Main | Description
---|---|---
800 | Clear | clear sky
801 | Clouds | few clouds: 11-25%
802 | Clouds | scattered clouds: 25-50%
803 | Clouds | broken clouds: 51-84%
804 | Clouds | overcast clouds: 85-100%


## Example for DemonOpenWeather.json

```javascript
{
  "Latitude": <Latitude>,
  "Longitude": <Longitude>,
  "AppID": "<Application ID>",
  "Language": "nl"
}
```
