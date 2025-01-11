using System.ComponentModel.DataAnnotations;
using Eshop.Domain.Models;

namespace Eshop.Domain;

public class Order()
{
    [Key]
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderLine> OrderItems { get; set; }
    public OrderStatus Status { get; set; }
    public enum OrderStatus
    {
        Nová,
        Zaplacena,
        Zrušena
    }
}
