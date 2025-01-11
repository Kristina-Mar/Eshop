using System.ComponentModel.DataAnnotations;

namespace Eshop.Domain.Models
{
    public class OrderLine
    {
        [Key]
        public int OrderLineId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double ItemPrice { get; set; }
    }
}