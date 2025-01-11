using Eshop.Domain.Models;

namespace Eshop.Domain.DTOs;

public class OrderLineGetResponseDto
{
    public int OrderLineId { get; set; }
    public string ItemName { get; set; }
    public int NumberOfItems { get; set; }
    public double ItemPrice { get; set; }

    public static OrderLineGetResponseDto FromDomain(OrderLine orderItem)
    {
        return new OrderLineGetResponseDto()
        {
            OrderLineId = orderItem.OrderLineId,
            ItemName = orderItem.ItemName,
            NumberOfItems = orderItem.Quantity,
            ItemPrice = orderItem.ItemPrice
        };
    }
}
