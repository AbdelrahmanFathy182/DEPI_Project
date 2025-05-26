using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHomeDashboard.Models
{

    public enum RoomType
    {
        Bedroom,
        LivingRoom,
        Kitchen,
        Bathroom,
        Office
    }
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public RoomType Type { get; set; }

        
        public DateTime Created_at { get; set; }
        [ForeignKey("account")]
        public int AccountId { get; set; }

        public Account account { get; set; }

        public ICollection<Device> Devices { get; set; }
    }
}
