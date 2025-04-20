using System.Text.Json.Serialization;

namespace OrderingService.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; } = null!;
    }
}