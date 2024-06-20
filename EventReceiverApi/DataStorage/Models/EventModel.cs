using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventReceiverApi.DataStorage.Models
{
    public class EventModel
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        [JsonIgnore]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
