using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SmartHomeDashboard.Models
{
    public enum DeviceType
    {
        Fan,
        Light,
        Thermostat
    }
    public class Device
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created_at { get; set; }
        public bool Favourite { get; set; }

        public DeviceType deviceType { get; set; }

        [Range(0, 100, ErrorMessage = "Device intensity must be between 0 and 100.")]
        public int Value { get; set; }

        [ForeignKey("room")]  //many to one
        public int RoomId { get; set; }
        [ValidateNever]
        public Room room { get; set; }
        [ValidateNever]
        public ICollection<ActionLog> actions { get; set; }
    }
}
