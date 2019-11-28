namespace KMNI.Models
{
  /// <summary>
  /// The list of [station] can be used to translate station code to a station name.
  /// The LocationCoord will be a manual action.
  /// Example B: 51° 58' N.B., L: 04° 55' O.L.
  /// </summary>
  public class Station
  {
    public int StationCode { get; set; }
    public string StationName { get; set; }
    public bool Active { get; set; }
    public LocationCoord Location { get; set; }
  }

  public class LocationCoord
  {
    public string B { get; set; }
    public string L { get; set; }
  }
}