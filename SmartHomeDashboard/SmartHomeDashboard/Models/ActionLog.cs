using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHomeDashboard.Models
{

    public enum ActionType
    {
        TurnOn,
        TurnOff,
        Increase,
        Decrease
    }
    public enum TargetAttribute
    {
       Speed,
       Brightness,
       Temperature
    }
    public class ActionLog
    {
        [Key]
        public int Id { get; set; }
        
        public ActionType Type { get; set; }
        public int Value { get; set; }

        public DateTime Created_at { get; set; }

        public TargetAttribute TrgtAttribute { get; set; }

        [ForeignKey("device")]  //many to one
        public int DeviceId { get; set; }

        public Device device { get; set; }
    }
}
