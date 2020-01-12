using System;
using System.Globalization;
using System.Linq;

// Original source: https://gist.github.com/RichardD2/f6b08a5791b21ac77ce7
// Original Gist GitHub: https://gist.github.com/RichardD2
namespace Trinet.Core
{
  /// <summary>
  /// Represents a geographic location (latitude and longitude).
  /// </summary>
  public struct GeographicLocation : IEquatable<GeographicLocation>, IFormattable
  {
    private const double Epsilon = 0.000001;
    private readonly double _latitude;
    private readonly double _longitude;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeographicLocation"/> struct.
    /// </summary>
    /// <param name="latitude">
    /// The latitude.
    /// </param>
    /// <param name="longitude">
    /// The longitude.
    /// </param>
    public GeographicLocation(double latitude, double longitude)
    {
      _latitude = ClampCoordinate(latitude);
      _longitude = ClampCoordinate(longitude);
    }

    /// <summary>
    /// Gets or sets the latitude of this location.
    /// </summary>
    /// <value>
    /// The latitude of this location.
    /// Positive values indicate North latitudes;
    /// negative values indicate South latitudes.
    /// </value>
    public double Latitude
    {
      get { return _latitude; }
    }

    /// <summary>
    /// Gets or sets the longitude of this location.
    /// </summary>
    /// <value>
    /// The longitude of this location.
    /// Positive values indicate East longitudes;
    /// negative values indicate West longitudes.
    /// </value>
    public double Longitude
    {
      get { return _longitude; }
    }

    /// <summary>
    /// Gets or sets the longitude-West of this location.
    /// </summary>
    /// <value>
    /// The longitude-West of this location.
    /// Positive values indicate West longitudes;
    /// negative values indicate East longitudes.
    /// </value>
    internal double LongitudeWest
    {
      get { return -_longitude; }
    }

    private static string FormatCoordinate(double value, string suffixPositive, string suffixNegative, IFormatProvider provider, byte precision)
    {
      string suffix = (Math.Abs(value) < Epsilon) ? null : ((0D > value) ? suffixNegative : suffixPositive);
      value = Math.Abs(value);

      double degrees = Math.Truncate(value);
      value -= degrees;
      value *= 60;

      double minutes = Math.Truncate(value);
      value -= minutes;
      value *= 60;

      int partIndex = 0;
      var parts = new string[4];

      parts[partIndex++] = degrees.ToString("#,##0", provider) + "\u00b0";

      if (Math.Abs(minutes) > Epsilon)
      {
        parts[partIndex++] = minutes.ToString("#,##0", provider) + "\'";
      }
      if (Math.Abs(value) > Epsilon)
      {
        string format = "###0." + ((0 >= precision) ? string.Empty : new string('0', precision));
        parts[partIndex++] = value.ToString(format, provider) + "\"";
      }
      if (null != suffix)
      {
        parts[partIndex++] = suffix;
      }

      return string.Join(" ", parts, 0, partIndex);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="format">
    /// A format string which controls how this instance is formatted.
    /// Specify <see langword="null"/> to use the default format string (<c>G4</c>).
    /// </param>
    /// <param name="formatProvider">
    /// The <see cref="IFormatProvider"/> instance which indicates how values are formatted.
    /// Specify <see langword="null"/> to use the current thread's formatting info.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    /// <remarks>
    /// <para>Supported format strings are:</para>
    /// <list type="table">
    /// <item>
    /// <term>R</term>
    /// <description>
    /// Round-trip format. 
    /// </description>
    /// </item>
    /// <item>
    /// <term>F</term>
    /// <description>
    /// Fixed precision format.
    /// Precision can be specified within the format string (eg: F6).
    /// If not specified, the default precision is 6.
    /// </description>
    /// </item>
    /// <item>
    /// <term>E</term>
    /// <description>
    /// Scientific format.
    /// Precision can be specified within the format string (eg: E6).
    /// If not specified, the default precision is 6.
    /// </description>
    /// </item>
    /// <item>
    /// <term>G</term>
    /// <description>
    /// Geographic format (degrees, minutes and seconds).
    /// Precision of the seconds can be specified within the format string (eg: G6).
    /// If not specified, the default precision is 4.
    /// </description>
    /// </item>
    /// </list>
    /// <para>If any other format string is specified, it defaults to <c>G4</c>.</para>
    /// </remarks>
    public string ToString(string format, IFormatProvider formatProvider)
    {
      if (formatProvider == null) formatProvider = CultureInfo.CurrentCulture;

      byte precision = 4;
      if (!string.IsNullOrEmpty(format))
      {
        if ('F' == format[0] || 'f' == format[0])
        {
          if (1 == format.Length) format = "F6";
          return Latitude.ToString(format, formatProvider)
              + ", " + Longitude.ToString(format, formatProvider);
        }
        if ('E' == format[0] || 'e' == format[0])
        {
          return Latitude.ToString(format, formatProvider)
              + ", " + Longitude.ToString(format, formatProvider);
        }
        if (string.Equals("R", format, StringComparison.OrdinalIgnoreCase))
        {
          return Latitude.ToString(format, formatProvider)
              + ", " + Longitude.ToString(format, formatProvider);
        }
        if ((format[0] == 'G' || format[0] == 'g') && format.Length != 1)
        {
          if (!byte.TryParse(format.Substring(1), NumberStyles.Integer, formatProvider, out precision))
          {
            precision = 4;
          }
        }
      }

      return FormatCoordinate(Latitude, "N", "S", formatProvider, precision)
          + ", " + FormatCoordinate(Longitude, "E", "W", formatProvider, precision);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="formatProvider">
    /// The <see cref="IFormatProvider"/> instance which indicates how values are formatted.
    /// Specify <see langword="null"/> to use the current thread's formatting info.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance,
    /// represented as degrees, minutes and seconds, with 4 digits of precision for the seconds.
    /// </returns>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="format">
    /// A format string which controls how this instance is formatted.
    /// Specify <see langword="null"/> to use the default format string (<c>G4</c>).
    /// </param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    /// <remarks>
    /// <para>Supported format strings are:</para>
    /// <list type="table">
    /// <item>
    /// <term>R</term>
    /// <description>
    /// Round-trip format. 
    /// </description>
    /// </item>
    /// <item>
    /// <term>F</term>
    /// <description>
    /// Fixed precision format.
    /// Precision can be specified within the format string (eg: F6).
    /// If not specified, the default precision is 6.
    /// </description>
    /// </item>
    /// <item>
    /// <term>E</term>
    /// <description>
    /// Scientific format.
    /// Precision can be specified within the format string (eg: E6).
    /// If not specified, the default precision is 6.
    /// </description>
    /// </item>
    /// <item>
    /// <term>G</term>
    /// <description>
    /// Geographic format (degrees, minutes and seconds).
    /// Precision of the seconds can be specified within the format string (eg: G6).
    /// If not specified, the default precision is 4.
    /// </description>
    /// </item>
    /// </list>
    /// <para>If any other format string is specified, it defaults to <c>G4</c>.</para>
    /// </remarks>
    public string ToString(string format)
    {
      return ToString(format, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance,
    /// represented as degrees, minutes and seconds, with 4 digits of precision for the seconds.
    /// </returns>
    public override string ToString()
    {
      return ToString(null, CultureInfo.CurrentCulture);
    }

    private static double ClampCoordinate(double value)
    {
      value = value % 360;
      if (value > 180D)
      {
        value -= 360D;
      }
      else if (value <= -180D)
      {
        value += 360D;
      }

      return value;
    }

    private static double? ParseCoordinate(string value, IFormatProvider provider)
    {
      double? result = null;
      if (value.Length != 0)
      {
        string degrees = null, minutes = null, seconds = null;
        var buffer = new char[value.Length];
        int bufferIndex = 0;

        foreach (char c in value)
        {
          switch (c)
          {
            case '\u00b0':
              {
                if (degrees != null) return null;
                degrees = (bufferIndex == 0) ? string.Empty : new string(buffer, 0, bufferIndex);
                bufferIndex = 0;
                break;
              }
            case '\u0027':
            case '\u02b9':
            case '\u2032':
              {
                if (minutes != null) return null;
                minutes = (bufferIndex == 0) ? string.Empty : new string(buffer, 0, bufferIndex);
                bufferIndex = 0;
                break;
              }
            case '\u0022':
            case '\u02ba':
            case '\u2033':
              {
                if (seconds != null) return null;
                seconds = (bufferIndex == 0) ? string.Empty : new string(buffer, 0, bufferIndex);
                bufferIndex = 0;
                break;
              }
            case '.':
            case '+':
            case '-':
              {
                buffer[bufferIndex++] = c;
                break;
              }
            case 'E':
            case 'e':
              {
                buffer[bufferIndex++] = 'E';
                break;
              }
            default:
              {
                if ('0' <= c && c <= '9')
                {
                  buffer[bufferIndex++] = c;
                }
                else if (!char.IsWhiteSpace(c))
                {
                  return null;
                }
                break;
              }
          }
        }

        if (0 != bufferIndex)
        {
          if (degrees == null)
          {
            degrees = new string(buffer, 0, bufferIndex);
          }
          else if (minutes == null)
          {
            minutes = new string(buffer, 0, bufferIndex);
          }
          else if (seconds == null)
          {
            seconds = new string(buffer, 0, bufferIndex);
          }
        }

        if (!string.IsNullOrEmpty(degrees))
        {
          double d;
          if (!double.TryParse(degrees, NumberStyles.Number | NumberStyles.AllowExponent, provider, out d))
          {
            return null;
          }

          result = d;
        }
        else
        {
          result = 0D;
        }
        if (!string.IsNullOrEmpty(minutes))
        {
          double m;
          if (!double.TryParse(minutes, NumberStyles.Number | NumberStyles.AllowExponent, provider, out m))
          {
            return null;
          }

          result += m / 60D;
        }
        if (!string.IsNullOrEmpty(seconds))
        {
          double s;
          if (!double.TryParse(seconds, NumberStyles.Number | NumberStyles.AllowExponent, provider, out s))
          {
            return null;
          }

          result += s / 3600D;
        }
      }

      return result;
    }

    /// <summary>
    /// Attempts to parse the string as a geographic location.
    /// </summary>
    /// <param name="value">
    /// The string to parse.
    /// </param>
    /// <param name="culture">
    /// A <see cref="CultureInfo"/> which determines how values are parsed.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the parsed <see cref="GeographicLocation"/>, if available.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the value was parsed;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParse(string value, CultureInfo culture, out GeographicLocation result)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        result = default(GeographicLocation);
        return false;
      }

      if (culture == null) culture = CultureInfo.CurrentCulture;
      var parts = value.Split(new[] { culture.TextInfo.ListSeparator }, StringSplitOptions.RemoveEmptyEntries).Take(3).ToArray();
      if (parts.Length != 2)
      {
        result = default(GeographicLocation);
        return false;
      }

      double? latitude = null;
      double? longitude = null;
      foreach (var part in parts.Select(p => p.Trim()))
      {
        if (part.EndsWith("N", StringComparison.OrdinalIgnoreCase))
        {
          if (latitude != null)
          {
            result = default(GeographicLocation);
            return false;
          }

          latitude = ParseCoordinate(part.Substring(0, part.Length - 1), culture);
          if (latitude == null)
          {
            result = default(GeographicLocation);
            return false;
          }
        }
        else if (part.EndsWith("S", StringComparison.OrdinalIgnoreCase))
        {
          if (latitude != null)
          {
            result = default(GeographicLocation);
            return false;
          }

          latitude = -ParseCoordinate(part.Substring(0, part.Length - 1), culture);
          if (latitude == null)
          {
            result = default(GeographicLocation);
            return false;
          }
        }
        else if (part.EndsWith("W", StringComparison.OrdinalIgnoreCase))
        {
          if (longitude != null)
          {
            result = default(GeographicLocation);
            return false;
          }

          longitude = -ParseCoordinate(part.Substring(0, part.Length - 1), culture);
          if (longitude == null)
          {
            result = default(GeographicLocation);
            return false;
          }
        }
        else if (part.EndsWith("E", StringComparison.OrdinalIgnoreCase))
        {
          if (longitude != null)
          {
            result = default(GeographicLocation);
            return false;
          }

          longitude = ParseCoordinate(part.Substring(0, part.Length - 1), culture);
          if (longitude == null)
          {
            result = default(GeographicLocation);
            return false;
          }
        }
        else
        {
          if (latitude == null)
          {
            latitude = ParseCoordinate(part, culture);
            if (latitude == null)
            {
              result = default(GeographicLocation);
              return false;
            }
          }
          else if (longitude == null)
          {
            longitude = ParseCoordinate(part, culture);
            if (longitude == null)
            {
              result = default(GeographicLocation);
              return false;
            }
          }
          else
          {
            result = default(GeographicLocation);
            return false;
          }
        }
      }

      if (latitude == null || longitude == null)
      {
        result = default(GeographicLocation);
        return false;
      }

      result = new GeographicLocation(latitude.Value, longitude.Value);
      return true;
    }

    /// <summary>
    /// Attempts to parse the string as a geographic location.
    /// </summary>
    /// <param name="value">
    /// The string to parse.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the parsed <see cref="GeographicLocation"/>, if available.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the value was parsed;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParse(string value, out GeographicLocation result)
    {
      return TryParse(value, CultureInfo.CurrentCulture, out result);
    }

    /// <summary>
    /// Parses the string as a geographic location.
    /// </summary>
    /// <param name="value">
    /// The string to parse.
    /// </param>
    /// <param name="culture">
    /// A <see cref="CultureInfo"/> which determines how values are parsed.
    /// </param>
    /// <returns>
    /// The <see cref="GeographicLocation"/> parsed from the string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="value"/> is <see langword="null"/> or <see cref="string.Empty"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="value"/> consists entirely of white-space.
    /// </exception>
    /// <exception cref="FormatException">
    /// <paramref name="value"/> is not a valid geographic location.
    /// </exception>
    public static GeographicLocation Parse(string value, CultureInfo culture)
    {
      if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");

      GeographicLocation result;
      if (TryParse(value, culture, out result)) return result;
      throw new FormatException();
    }

    /// <summary>
    /// Parses the string as a geographic location.
    /// </summary>
    /// <param name="value">
    /// The string to parse.
    /// </param>
    /// <returns>
    /// The <see cref="GeographicLocation"/> parsed from the string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="value"/> is <see langword="null"/> or <see cref="string.Empty"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="value"/> consists entirely of white-space.
    /// </exception>
    /// <exception cref="FormatException">
    /// <paramref name="value"/> is not a valid geographic location.
    /// </exception>
    public static GeographicLocation Parse(string value)
    {
      return Parse(value, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer that is the hash code for this instance.
    /// </returns>
    public override int GetHashCode()
    {
      unchecked
      {
        return (_latitude.GetHashCode() * 397) ^ _longitude.GetHashCode();
      }
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">
    /// Another object to compare to. 
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="obj"/> and this instance are the same type and represent the same value;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (!(obj is GeographicLocation)) return false;
      return Equals((GeographicLocation)obj);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">
    /// An object to compare with this object.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(GeographicLocation other)
    {
      return Math.Abs(other._latitude - this._latitude) < Epsilon
          && Math.Abs(other._longitude - this._longitude) < Epsilon;
    }

    /// <summary>
    /// Implements the equality operator.
    /// </summary>
    /// <param name="left">
    /// The left operand.
    /// </param>
    /// <param name="right">
    /// The right operand.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(GeographicLocation left, GeographicLocation right)
    {
      return left.Equals(right);
    }

    /// <summary>
    /// Implements the inequality operator.
    /// </summary>
    /// <param name="left">
    /// The left operand.
    /// </param>
    /// <param name="right">
    /// The right operand.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(GeographicLocation left, GeographicLocation right)
    {
      return !left.Equals(right);
    }
  }
}