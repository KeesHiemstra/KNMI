// Compute times of moonrise and moonset at a specified latitude and longitude.
//
// This software minimizes computational work by performing the full calculation
// of the lunar position three times, at the beginning, middle, and end of the
// period of interest.  Three point interpolation is used to predict the position
// for each hour, and the arithmetic mean is used to predict the half-hour positions.
//
// The full computational burden is negligible on modern computers, but the
// algorithm is effective and still useful for small embedded systems.
//
// This software was originally adapted to javascript by Stephen R. Schmitt
// from a BASIC program from the 'Astronomical Computing' column of Sky & Telescope,
// July 1989, page 78.
//
// Subsequently adapted from Stephen R. Schmitt's javascript to c++ for the Arduino
// by Cyrus Rahman, this work is subject to Stephen Schmitt's copyright:
//
// Copyright 2007 Stephen R. Schmitt  
// Subsequent work Copyright 2020-2026 Cyrus Rahman
// You may use or modify this source code in any way you find useful, provided
// that you agree that the author(s) have no warranty, obligations or liability.  You
// must determine the suitability of this source code for your use.
//
// Redistributions of this source code must retain this copyright notice.

using System;

namespace WeatherMonitor.Models
{
	internal class MoonRise
	{
		public const double MR_WINDOW = 48;

		public struct skyCoordinates
		{
			public double RA;            // Right ascension
			public double declination;   // Declination
			public double distance;      // Distance
		};

		public DateTime queryTime;
		public DateTime RiseTime;
		public DateTime SetTime;
		public float riseAz;
		public float setAz;
		public bool hasRise;
		public bool hasSet;
		public bool isVisible;

		public MoonRise()
		{
			queryTime = DateTime.MinValue;
			RiseTime = DateTime.MinValue;
			SetTime = DateTime.MinValue;
			riseAz = 0;
			setAz = 0;
			hasRise = false;
			hasSet = false;
			isVisible = false;
		}

		// Determine the nearest moon rise or set event previous, and the nearest
		// moon rise or set event subsequent, to the specified time in seconds since the
		// Unix epoch (January 1, 1970) and at the specified latitude and longitude in
		// degrees.
		//
		// We look for events from MR_WINDOW/2 hours in the past to MR_WINDOW/2 hours
		// in the future.
		public void Calculate(double latitude, double longitude, DateTime t)
		{
			skyCoordinates[] moonPosition = new skyCoordinates[3];
			double offsetDays;

			queryTime = t;
			offsetDays = JulianDate(t) - 2451545L;     // Days since Jan 1, 2000, 1200UTC.
																								 // Begin testing (MR_WINDOW / 2) hours before
																								 // requested time.
			offsetDays -= MR_WINDOW / (2 * 24);

			// Calculate coordinates at start, middle, and end of search period.
			for (int i = 0; i < 3; i++)
			{
				moonPosition[i] = Moon(offsetDays + i * MR_WINDOW / (2 * 24));
			}

			// If the RA wraps around during this period, unwrap it to keep the
			// sequence smooth for interpolation.
			if (moonPosition[1].RA <= moonPosition[0].RA)
			{
				moonPosition[1].RA += 2 * Math.PI;
			}

			if (moonPosition[2].RA <= moonPosition[1].RA)
			{
				moonPosition[2].RA += 2 * Math.PI;
			}

			// Initialize interpolation array.
			skyCoordinates[] mpWindow = new skyCoordinates[3];
			mpWindow[0].RA = moonPosition[0].RA;
			mpWindow[0].declination = moonPosition[0].declination;
			mpWindow[0].distance = moonPosition[0].distance;

			for (int k = 0; k < MR_WINDOW; k++)
			{
				// Check each interval of search period
				float ph = (float)(k + 1) / (float)MR_WINDOW;

				mpWindow[2].RA = Interpolate(moonPosition[0].RA,
						 moonPosition[1].RA,
						 moonPosition[2].RA, ph);
				mpWindow[2].declination = Interpolate(moonPosition[0].declination,
								moonPosition[1].declination,
								moonPosition[2].declination, ph);
				mpWindow[2].distance = moonPosition[2].distance;

				// Look for moonrise/set events during this interval.
				TestMoonRiseSet(k, offsetDays, latitude, longitude, mpWindow);

				mpWindow[0] = mpWindow[2]; // Advance to next interval.
			}
		}

		private float K1()
		{
			// K1 15*(M_PI/180)*1.0027379
			return (float)(15 * (Math.PI / 180) * 1.0027379);
		}
		private double Remainder(double x, double y)
		{
			//remainder(x, y) ((double)((double)x - (double)y * rint((double)x / (double)y)))
			return x - y * (int)(x / y);
		}

		// Look for moon rise and set events during an hour.
		private void TestMoonRiseSet(int k, double offsetDays, double latitude, double longitude, skyCoordinates[] mp)
		{
			double[] ha = new double[3], VHz = new double[3];
			double lSideTime;

			// Get (local_sidereal_time - MR_WINDOW / 2) hours in radians.
			lSideTime = LocalSiderealTime(offsetDays, longitude) * 2 * Math.PI / 360;

			// Calculate Hour Angle.
			ha[0] = lSideTime - mp[0].RA + (double)k * K1();
			ha[2] = lSideTime - mp[2].RA + (double)k * K1() + K1();

			// Hour Angle and declination at half hour.
			ha[1] = (ha[2] + ha[0]) / 2;
			mp[1].declination = (mp[2].declination + mp[0].declination) / 2;

			double s = Math.Sin(Math.PI / 180 * latitude);
			double c = Math.Cos(Math.PI / 180 * latitude);

			// refraction + semidiameter at horizon + distance correction
			double z = Math.Cos(Math.PI / 180 * (90.567 - 41.685 / mp[0].distance));

			// Combine corrections into a vertical unit sphere length.
			VHz[0] = s * Math.Sin(mp[0].declination) +
							 c * Math.Cos(mp[0].declination) * Math.Cos(ha[0]) - z;
			VHz[2] = s * Math.Sin(mp[2].declination) +
							 c * Math.Cos(mp[2].declination) * Math.Cos(ha[2]) - z;

			if (Math.Sign(VHz[0]) == Math.Sign(VHz[2]))
				goto noevent; // No event this hour.

			VHz[1] = s * Math.Sin(mp[1].declination) +
							 c * Math.Cos(mp[1].declination) * Math.Cos(ha[1]) - z;

			// Use quadratic formula to invert the quadratic interpolation.
			double a, b, d, e, time;
			a = 2 * VHz[2] - 4 * VHz[1] + 2 * VHz[0];
			b = 4 * VHz[1] - 3 * VHz[0] - VHz[2];
			d = b * b - 4 * a * VHz[0];

			// Switch to linear interpolation if a is too small.  This unusual situation
			// can arise if the rise/set occurs at the midpoint of the test interval (ha[1])
			// and will lead to a division by zero.
			// (found by Claude.ai)
			if (Math.Abs(a) < 1e-6)
			{
				// Switch to linear interpolation.
				e = -VHz[0] / (VHz[2] - VHz[0]);
			}
			else
			{
				if (d < 0)
				{
					// This probably never happens.
					goto noevent;
				}

				d = Math.Sqrt(d);
				e = (-b + d) / (2 * a);
				if ((e < 0) || (e > 1))
				{
					e = (-b - d) / (2 * a);
				}
			}
			time = k + e + 1.0 / 120; // Round off. Time since k=0 of event (in hours).

			// The time we started searching + the time from the start of the search to the
			// event is the time of the event.  Add (time since k=0) - window/2 hours.
			DateTime eventTime;
			eventTime = queryTime + TimeSpan.FromHours((time - MR_WINDOW / 2));

			double hz, nz, dz, az;
			hz = ha[0] + e * (ha[2] - ha[0]);     // Azimuth of the moon at the event.
			nz = -Math.Cos(mp[1].declination) * Math.Sin(hz);
			dz = c * Math.Sin(mp[1].declination) - s * Math.Cos(mp[1].declination) * Math.Cos(hz);
			az = Math.Atan2(nz, dz) / (Math.PI / 180);
			if (az < 0)
			{
				az += 360;
			}

			// If there is no previously recorded event of this type, save this event.
			//
			// If this event is previous to queryTime, and is the nearest event to queryTime
			// of events of its type previous to queryType, save this event, replacing the
			// previously recorded event of its type.  Events subsequent to queryTime are
			// treated similarly, although since events are tested in chronological order
			// no replacements will occur as successive events will be further from
			// queryTime.
			//
			// If this event is subsequent to queryTime and there is an event of its type
			// previous to queryTime, then there is an event of the other type between the
			// two events of this event's type.  If the event of the other type is
			// previous to queryTime, then it is the nearest event to queryTime that is
			// previous to queryTime.  In this case save the current event, replacing
			// the previously recorded event of its type.  Otherwise discard the current
			// event.
			//
			if ((VHz[0] < 0) && (VHz[2] > 0))
			{
				if (!hasRise ||
						((RiseTime < queryTime) == (eventTime < queryTime) &&
						Math.Abs(RiseTime.Ticks - queryTime.Ticks) > Math.Abs(eventTime.Ticks - queryTime.Ticks)) ||
						((RiseTime < queryTime) != (eventTime < queryTime) &&
						 (hasSet &&
						(RiseTime < queryTime) == (SetTime < queryTime))))
				{
					RiseTime = eventTime;
					riseAz = (float)az;
					hasRise = true;
				}
			}

			if ((VHz[0] > 0) && (VHz[2] < 0))
			{
				if (!hasSet ||
					 ((SetTime < queryTime) == (eventTime < queryTime) &&
					 Math.Abs(SetTime.Ticks - queryTime.Ticks) > Math.Abs(eventTime.Ticks - queryTime.Ticks)) ||
					 ((SetTime < queryTime) != (eventTime < queryTime) &&
					 (hasRise &&
					 (SetTime < queryTime) == (RiseTime < queryTime))))
				{
					SetTime = eventTime;
					setAz = (float)az;
					hasSet = true;
				}
			}

		noevent:
			// There are obscure cases in the polar regions that require extra logic.
			if (!hasRise && !hasSet)
				isVisible = !(Math.Sign(VHz[2]) < 0);
			else if (hasRise && !hasSet)
				isVisible = (queryTime > RiseTime);
			else if (!hasRise && hasSet)
				isVisible = (queryTime < SetTime);
			else
				isVisible = ((RiseTime < SetTime && RiseTime < queryTime && SetTime > queryTime) ||
				 (RiseTime > SetTime && (RiseTime < queryTime || SetTime > queryTime)));

			return;
		}

		// Moon position using fundamental arguments 
		// (Van Flandern & Pulkkinen, 1979)
		// c.f. Van Flandern & Pulkkinen, 1979, accurate within 1' in interval 1979 +/- 300 years
		private skyCoordinates Moon(double dayOffset)
		{
			double l = 0.606434 + 0.03660110129 * dayOffset;
			double m = 0.374897 + 0.03629164709 * dayOffset;
			double f = 0.259091 + 0.03674819520 * dayOffset;
			double d = 0.827362 + 0.03386319198 * dayOffset;
			double n = 0.347343 - 0.00014709391 * dayOffset;
			double g = 0.993126 + 0.00273777850 * dayOffset;

			l = 2 * Math.PI * (l - Math.Floor(l));
			m = 2 * Math.PI * (m - Math.Floor(m));
			f = 2 * Math.PI * (f - Math.Floor(f));
			d = 2 * Math.PI * (d - Math.Floor(d));
			n = 2 * Math.PI * (n - Math.Floor(n));
			g = 2 * Math.PI * (g - Math.Floor(g));

			double v, u, w;
			v = 0.39558 * Math.Sin(f + n)
				+ 0.08200 * Math.Sin(f)
				+ 0.03257 * Math.Sin(m - f - n)
				+ 0.01092 * Math.Sin(m + f + n)
				+ 0.00666 * Math.Sin(m - f)
				- 0.00644 * Math.Sin(m + f - 2 * d + n)
				- 0.00331 * Math.Sin(f - 2 * d + n)
				- 0.00304 * Math.Sin(f - 2 * d)
				- 0.00240 * Math.Sin(m - f - 2 * d - n)
				+ 0.00226 * Math.Sin(m + f)
				- 0.00108 * Math.Sin(m + f - 2 * d)
				- 0.00079 * Math.Sin(f - n)
				+ 0.00078 * Math.Sin(f + 2 * d + n);

			u = 1
				- 0.10828 * Math.Cos(m)
				- 0.01880 * Math.Cos(m - 2 * d)
				- 0.01479 * Math.Cos(2 * d)
				+ 0.00181 * Math.Cos(2 * m - 2 * d)
				- 0.00147 * Math.Cos(2 * m)
				- 0.00105 * Math.Cos(2 * d - g)
				- 0.00075 * Math.Cos(m - 2 * d + g);

			w = 0.10478 * Math.Sin(m)
				- 0.04105 * Math.Sin(2 * f + 2 * n)
				- 0.02130 * Math.Sin(m - 2 * d)
				- 0.01779 * Math.Sin(2 * f + n)
				+ 0.01774 * Math.Sin(n)
				+ 0.00987 * Math.Sin(2 * d)
				- 0.00338 * Math.Sin(m - 2 * f - 2 * n)
				- 0.00309 * Math.Sin(g)
				- 0.00190 * Math.Sin(2 * f)
				- 0.00144 * Math.Sin(m + n)
				- 0.00144 * Math.Sin(m - 2 * f - n)
				- 0.00113 * Math.Sin(m + 2 * f + 2 * n)
				- 0.00094 * Math.Sin(m - 2 * d + g)
				- 0.00092 * Math.Sin(2 * m - 2 * d);

			double s;
			skyCoordinates sc;
			s = w / Math.Sqrt(u - (v * v));

			sc.RA = l + Math.Atan(s / Math.Sqrt(1 - (s * s))); // Right ascension

			s = v / Math.Sqrt(u);
			sc.declination = Math.Atan(s / Math.Sqrt(1 - (s * s))); // Declination
			sc.distance = 60.40974 * Math.Sqrt(u); // Distance

			return sc;
		}

		// 3-point interpolation
		private double Interpolate(double f0, double f1, double f2, double p)
		{
			double a = f1 - f0;
			double b = f2 - f1 - a;
			return (f0 + p * (2 * a + b * (2 * p - 1)));
		}

		// Determine Julian date from Unix time.
		// Provides marginally accurate results with Arduino 4-byte double.
		private double JulianDate(DateTime t)
		{
			return (t.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds / 86400.0 + 2440587.5;
		}

		// Local Sidereal Time
		// Provides local sidereal time in degrees, requires longitude in degrees
		// and time in fractional Julian days since Jan 1, 2000, 1200UTC (e.g. the
		// Julian date - 2451545).
		// cf. USNO Astronomical Almanac and
		// https://astronomy.stackexchange.com/questions/24859/local-sidereal-time
		private double LocalSiderealTime(double offsetDays, double longitude)
		{
			double lSideTime = (15.0 * (6.697374558 + 0.06570982441908 * offsetDays +
												 Remainder(offsetDays, 1) * 24 + 12 +
												 0.000026 * (offsetDays / 36525) * (offsetDays / 36525)) +
												 longitude) / 360;
			lSideTime -= Math.Floor(lSideTime);
			lSideTime *= 360; // Convert to degrees.
			return (lSideTime);
		}

	}
}
