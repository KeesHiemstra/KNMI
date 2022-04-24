using System;
using System.Diagnostics;

// Original source: https://gist.github.com/RichardD2/f6b08a5791b21ac77ce7
// Original Gist GitHub: https://gist.github.com/RichardD2
namespace Trinet.Core
{
	/// <summary>
	/// Represents the time of sunrise and sunset for a date and location.
	/// </summary>
	/// <remarks>
	/// <para>The calculated times have a precision of approximately three minutes.
	/// Other factors, such as air temperature and altitude, may affect the times by up to five minutes.</para>
	/// 
	/// <para>At very high/low latitudes (close to the poles), there are days when the sun never rises (during the winter)
	/// or never sets (during the summer). In these cases, the <see cref="SunriseTimeUtc"/> and
	/// <see cref="SunsetTimeUtc"/> properties will return <see langword="null"/>.</para>
	/// 
	/// <para>The calculation was found at: <a href="http://users.electromagnetic.net/bu/astro/sunrise-set.php" target="_blank">users.electromagnetic.net/bu/astro/sunrise-set.php</a>.</para>
	/// <para>The constants are defined at: <a href="http://www.astro.uu.nl/~strous/AA/en/reken/zonpositie.html" target="_blank">www.astro.uu.nl/~strous/AA/en/reken/zonpositie.html</a></para>
	/// <para>See also: <a href="http://en.wikipedia.org/wiki/Sunrise_equation" target="_blank">en.wikipedia.org/wiki/Sunrise_equation</a></para>
	/// </remarks>
	/// <example>
	/// <code>
	/// var location = GeographicLocation.Parse("50° 49\' 55.8717\" N, 0° 17\' 45.2833\" E");
	/// var hours = DaylightHours.Calculate(2010, 1, 8, location);
	/// Console.WriteLine("On {0:D}, sun rises at {1:hh:mm tt} and sets at {2:hh:mm tt}", hours.Day, hours.SunriseUtc, hours.SunsetUtc);
	/// // Output: "On 08 January 2010, sun rises at 08:00 AM and sets at 04:12 PM"
	/// </code>
	/// </example>
	public sealed class DaylightHours
	{
		private static readonly DateTimeOffset BaseJulianDate = new DateTimeOffset(2000, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
		private const double BaseJulianDays = 2451545.0008;
		private const double EarthDegreesPerYear = 357.5291;
		private const double EarthDegreesPerDay = 0.98560028;
		private const double EarthEocCoeff1 = 1.9148;
		private const double EarthEocCoeff2 = 0.0200;
		private const double EarthEocCoeff3 = 0.0003;
		private const double EarthPerihelion = 102.9372;
		private const double EarthObliquity = 23.44;
		private const double EarthTransitJ1 = 0.0053;
		private const double EarthTransitJ2 = -0.0069;
		private const double EarthSolarDiskDiameter = -0.83;
		private const double Epsilon = 0.000001;

		private readonly GeographicLocation _location;
		private readonly DateTimeOffset _day;
		private readonly TimeSpan? _sunrise;
		private readonly TimeSpan? _sunset;

		/// <summary>
		/// Initializes a new instance of the <see cref="DaylightHours"/> class.
		/// </summary>
		/// <param name="location">
		/// The location.
		/// </param>
		/// <param name="day">
		/// The day.
		/// </param>
		/// <param name="sunrise">
		/// The sunrise time.
		/// </param>
		/// <param name="sunset">
		/// The sunset time.
		/// </param>
		private DaylightHours(GeographicLocation location, DateTimeOffset day, TimeSpan? sunrise, TimeSpan? sunset)
		{
			_day = day;
			_location = location;
			_sunrise = sunrise;
			_sunset = sunset;
		}

		/// <summary>
		/// Returns the geographic location for which these times were calculated.
		/// </summary>
		/// <value>
		/// The <see cref="GeographicLocation"/> for which these times were calculated.
		/// </value>
		public GeographicLocation Location
		{
			get { return _location; }
		}

		/// <summary>
		/// Returns the day for which these times were calculated.
		/// </summary>
		/// <value>
		/// The day for which these times were calculated.
		/// </value>
		public DateTimeOffset Day
		{
			get { return _day; }
		}

		/// <summary>
		/// Returns the sunrise time in UTC, if any.
		/// </summary>
		/// <value>
		/// The sunrise time in UTC, if any.
		/// </value>
		public TimeSpan? SunriseTimeUtc
		{
			get { return _sunrise; }
		}

		/// <summary>
		/// Returns the sunrise date and time in UTC, if any.
		/// </summary>
		/// <value>
		/// The sunrise date and time in UTC, if any.
		/// </value>
		public DateTimeOffset? SunriseUtc
		{
			get
			{
				if (_sunrise == null) return null;
				return _day + _sunrise.Value;
			}
		}

		/// <summary>
		/// Returns the sunset time in UTC, if any.
		/// </summary>
		/// <value>
		/// The sunset time in UTC, if any.
		/// </value>
		public TimeSpan? SunsetTimeUtc
		{
			get { return _sunset; }
		}

		/// <summary>
		/// Returns the sunset date and time in UTC, if any.
		/// </summary>
		/// <value>
		/// The sunset date and time in UTC, if any.
		/// </value>
		public DateTimeOffset? SunsetUtc
		{
			get
			{
				if (_sunset == null) return null;
				return _day + _sunset.Value;
			}
		}

		private static double DegreesToRadians(double degrees)
		{
			return (degrees / 180D) * Math.PI;
		}

		private static double RadiansToDegrees(double radians)
		{
			return (radians / Math.PI) * 180D;
		}

		private static double Sin(double degrees)
		{
			return Math.Sin(DegreesToRadians(degrees));
		}

		private static double Asin(double d)
		{
			return RadiansToDegrees(Math.Asin(d));
		}

		private static double Cos(double degrees)
		{
			return Math.Cos(DegreesToRadians(degrees));
		}

		private static double Acos(double d)
		{
			return RadiansToDegrees(Math.Acos(d));
		}

		private static double ToJulian(DateTimeOffset value)
		{
			return BaseJulianDays + (value - BaseJulianDate).TotalDays;
		}

		private static DateTimeOffset FromJulian(double value)
		{
			return BaseJulianDate.AddDays(value - BaseJulianDays).AddHours(12);
		}

		private static DaylightHours CalculateCore(DateTimeOffset day, GeographicLocation location)
		{
			double jdate = ToJulian(day);
			double n = Math.Round((jdate - BaseJulianDays - 0.0009D) - (location.LongitudeWest / 360D));
			double jnoon = BaseJulianDays + 0.0009D + (location.LongitudeWest / 360D) + n;
			double m = (EarthDegreesPerYear + (EarthDegreesPerDay * (jnoon - BaseJulianDays))) % 360;
			double c = (EarthEocCoeff1 * Sin(m)) + (EarthEocCoeff2 * Sin(2 * m)) + (EarthEocCoeff3 * Sin(3 * m));
			double sunLon = (m + EarthPerihelion + c + 180D) % 360;
			double jtransit = jnoon + (EarthTransitJ1 * Sin(m)) + (EarthTransitJ2 * Sin(2 * sunLon));

			int iteration = 0;
			double oldMean;
			do
			{
				oldMean = m;

				m = (EarthDegreesPerYear + (EarthDegreesPerDay * (jtransit - BaseJulianDays))) % 360;
				c = (EarthEocCoeff1 * Sin(m)) + (EarthEocCoeff2 * Sin(2 * m)) + (EarthEocCoeff3 * Sin(3 * m));
				sunLon = (m + EarthPerihelion + c + 180D) % 360;
				jtransit = jnoon + (EarthTransitJ1 * Sin(m)) + (EarthTransitJ2 * Sin(2 * sunLon));
			}
			while (iteration++ < 100 && Math.Abs(m - oldMean) > Epsilon);

			double sunDec = Asin(Sin(sunLon) * Sin(EarthObliquity));
			double h = Acos((Sin(EarthSolarDiskDiameter) - Sin(location.Latitude) * Sin(sunDec)) / (Cos(location.Latitude) * Cos(sunDec)));

			if (Math.Abs(h) < Epsilon || double.IsNaN(h) || double.IsInfinity(h))
			{
				return new DaylightHours(location, day, null, null);
			}

			double jnoon2 = BaseJulianDays + 0.0009D + ((h + location.LongitudeWest) / 360D) + n;
			double jset = jnoon2 + (EarthTransitJ1 * Sin(m)) + (EarthTransitJ2 * Sin(2 * sunLon));
			double jrise = jtransit - (jset - jtransit);

			DateTimeOffset sunrise = FromJulian(jrise);
			Debug.Assert(sunrise.Date == day.Date, "Wrong sunrise", "Sunrise: Expected {0:D} but got {1:D} instead.", day, sunrise.Date);

			DateTimeOffset sunset = FromJulian(jset);
			Debug.Assert(sunset.Date == day.Date, "Wrong sunset", "Sunset: Expected {0:D} but got {1:D} instead.", day, sunset.Date);

			return new DaylightHours(location, day, sunrise.TimeOfDay, sunset.TimeOfDay);
		}

		/// <summary>
		/// Calculates the sunrise and sunset days for the specified date and location.
		/// </summary>
		/// <param name="day">
		/// The date for which the times are calculated.
		/// </param>
		/// <param name="location">
		/// The <see cref="GeographicLocation"/> for which the times are calculated.
		/// </param>
		/// <returns>
		/// A <see cref="DaylightHours"/> instance representing the calculated times.
		/// This value will never be <see langword="null"/>, although the sunrise/sunset times could be.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// There was an error with the calculation.
		/// </exception>
		public static DaylightHours Calculate(DateTimeOffset day, GeographicLocation location)
		{
			return CalculateCore(day.UtcDateTime.Date, location);
		}

		/// <summary>
		/// Calculates the sunrise and sunset days for the specified date and location.
		/// </summary>
		/// <param name="day">
		/// The date for which the times are calculated.
		/// </param>
		/// <param name="location">
		/// The <see cref="GeographicLocation"/> for which the times are calculated.
		/// </param>
		/// <returns>
		/// A <see cref="DaylightHours"/> instance representing the calculated times.
		/// This value will never be <see langword="null"/>, although the sunrise/sunset times could be.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// There was an error with the calculation.
		/// </exception>
		public static DaylightHours Calculate(DateTime day, GeographicLocation location)
		{
			return CalculateCore(day.ToUniversalTime().Date, location);
		}

		/// <summary>
		/// Calculates the sunrise and sunset days for the specified date and location.
		/// </summary>
		/// <param name="year">
		/// The year for which the times are calculated.
		/// </param>
		/// <param name="month">
		/// The month for which the times are calculated.
		/// </param>
		/// <param name="day">
		/// The day for which the times are calculated.
		/// </param>
		/// <param name="location">
		/// The <see cref="GeographicLocation"/> for which the times are calculated.
		/// </param>
		/// <returns>
		/// A <see cref="DaylightHours"/> instance representing the calculated times.
		/// This value will never be <see langword="null"/>, although the sunrise/sunset times could be.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="year"/> is less than 1 or greater than 9999.</para>
		/// <para>-or-</para>
		/// <para><paramref name="month"/> is less than 1 or greater than 12.</para>
		/// <para>-or-</para>
		/// <para><paramref name="day"/> is less than 1 or greater than the number of days in the specified month.</para>
		/// <para>-or-</para>
		/// <para>The specified date is earlier than <see cref="DateTimeOffset.MinValue"/> or later than <see cref="DateTimeOffset.MaxValue"/>.</para>
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// There was an error with the calculation.
		/// </exception>
		public static DaylightHours Calculate(int year, int month, int day, GeographicLocation location)
		{
			var date = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
			return CalculateCore(date, location);
		}
	}
}