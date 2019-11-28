using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMNI.Models
{
  [Table("KNMI_Station")]
  public class Station
  {
    [Key]
    public int StationCode { get; set; }

    [Required]
    public string StationName { get; set; }
    
    [Required]
    public bool IsActive { get; set; }
  }
}