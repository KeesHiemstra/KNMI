using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMNI.Models
{
  /*********
  * SOURCE: ROYAL NETHERLANDS METEOROLOGICAL INSTITUTE (KNMI)
  * Comment: These time series are inhomogeneous because of station relocations and changes in observation 
  * techniques. As a result, these series are not suitable for trend analysis. For climate change studies 
  * we refer to the homogenized series of monthly temperatures of 
  * De Bilt <http://www.knmi.nl/kennis-en-datacentrum/achtergrond/gehomogeniseerde-reeks-maandtemperaturen-de-bilt> 
  * or the Central Netherlands Temperature <http://www.knmi.nl/kennis-en-datacentrum/achtergrond/centraal-nederland-temperatuur-cnt>.
  *********/

  [Table("KNMI_Daily")]
  public class DailyReport
  {

    [Key]
    public int Id { get; set; }

    // STN      = Station number
    [Required]
    public int Stn { get; set; }

    // YYYYMMDD = Date (YYYY = year MM = month DD = day)
    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    // DDVEC    = Vector mean wind direction in degrees (360=north, 90=east, 180=south, 270=west, 0=calm/variable)
    public int? DDVec { get; set; }

    // FHVEC    = Vector mean wind speed (in m/s)
    public decimal? FHVec { get; set; }

    // FG       = Daily mean wind speed (in m/s)
    public decimal? FG { get; set; }

    // FHX      = Maximum hourly mean wind speed (in m/s)
    public decimal? FHX { get; set; }

    // FHXH     = Hourly division in which FHX was measured
    public int? FHXH { get; set; }

    // FHN      = Minimum hourly mean wind speed (in m/s)
    public decimal? FHN { get; set; }

    // FHNH     = Hourly division in which FHN was measured
    public int? FHNH { get; set; }

    // FXX      = Maximum wind gust (in m/s)
    public decimal? FXX { get; set; }

    // FXXH     = Hourly division in which FXX was measured
    public int? FXXH { get; set; }

    // TG       = Daily mean temperature in (in degrees Celsius)
    public decimal? TG { get; set; }

    // TN       = Minimum temperature (in degrees Celsius)
    public decimal? TN { get; set; }

    // TNH      = Hourly division in which TN was measured
    public int? TNH { get; set; }

    // TX       = Maximum temperature (in degrees Celsius)
    public decimal? TX { get; set; }

    // TXH      = Hourly division in which TX was measured
    public int? TXH { get; set; }

    // T10N     = Minimum temperature at 10 cm above surface (in degrees Celsius)
    public decimal? T10N { get; set; }

    // T10NH    = 6-hourly division in which T10N was measured; 6=0-6 UT, 12=6-12 UT, 18=12-18 UT, 24=18-24 UT
    public int? T10NH { get; set; }

    // SQ       = Sunshine duration (in hour) calculated from global radiation (-1 for <0.05 hour)
    public decimal? SQ { get; set; }

    // SP       = Percentage of maximum potential sunshine duration
    public int? SP { get; set; }

    // Q        = Global radiation (in J/cm2)
    public int? Q { get; set; }

    // DR       = Precipitation duration (in hour)
    public decimal? DR { get; set; }

    // RH       = Daily precipitation amount (in mm) (-1 for <0.05 mm)
    public decimal? RH { get; set; }

    // RHX      = Maximum hourly precipitation amount (in mm) (-1 for <0.05 mm)
    public decimal? RHX { get; set; }

    // RHXH     = Hourly division in which RHX was measured
    public int? RHXH { get; set; }

    // PG       = Daily mean sea level pressure (in hPa) calculated from 24 hourly values
    public decimal? PG { get; set; }

    // PX       = Maximum hourly sea level pressure (in hPa)
    public decimal? PX { get; set; }

    // PXH      = Hourly division in which PX was measured
    public int? PXH { get; set; }

    // PN       = Minimum hourly sea level pressure (in hPa)
    public decimal? PN { get; set; }

    // PNH      = Hourly division in which PN was measured
    public int? PNH { get; set; }

    // VVN      = Minimum visibility; 0: <100 m, 1:100-200 m, 2:200-300 m,..., 49:4900-5000 m, 50:5-6 km, 56:6-7 km, 57:7-8 km,..., 79:29-30 km, 80:30-35 km, 81:35-40 km,..., 89: >70 km)
    public int? VVN { get; set; }

    // VVNH     = Hourly division in which VVN was measured
    public int? VVNH { get; set; }

    // VVX      = Maximum visibility; 0: <100 m, 1:100-200 m, 2:200-300 m,..., 49:4900-5000 m, 50:5-6 km, 56:6-7 km, 57:7-8 km,..., 79:29-30 km, 80:30-35 km, 81:35-40 km,..., 89: >70 km)
    public int? VVX { get; set; }

    // VVXH     = Hourly division in which VVX was measured
    public int? VVXH { get; set; }

    // NG       = Mean daily cloud cover (in octants, 9=sky invisible)
    public int? NG { get; set; }

    // UG       = Daily mean relative atmospheric humidity (in percents)
    public int? UG { get; set; }

    // UX       = Maximum relative atmospheric humidity (in percents)
    public int? UX { get; set; }

    // UXH      = Hourly division in which UX was measured
    public int? UXH { get; set; }

    // UN       = Minimum relative atmospheric humidity (in percents)
    public int? UN { get; set; }

    // UNH      = Hourly division in which UN was measured
    public int? UNH { get; set; }

    // EV24     = Potential evapotranspiration (Makkink) (in mm)
    public decimal? EV24 { get; set; }

  }
}
