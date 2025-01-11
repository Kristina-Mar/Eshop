using Eshop.Domain.Models;

namespace Eshop.Domain.DTOs
{
    public class OrderLineCreateRequestDto(string ItemName, int NumberOfItems, double ItemPrice)
    {
        public string ItemName { get; set; } = ItemName;
        public int NumberOfItems { get; set; } = NumberOfItems;
        public double ItemPrice { get; set; } = ItemPrice;
        public OrderLine ToDomain() => new() { ItemName = ItemName, NumberOfItems = NumberOfItems, ItemPrice = ItemPrice };
    }
}