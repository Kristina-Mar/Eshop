namespace Eshop.Domain.DTOs
{
    public class OrderCreateRequestDto(string CustomerName, DateTime OrderDate, List<OrderLineCreateRequestDto> OrderItemsDtoList)
    {
        public string CustomerName { get; set; } = CustomerName;
        public DateTime OrderDate { get; set; } = OrderDate;
        public List<OrderLineCreateRequestDto> OrderItemsDtoList { get; set; } = OrderItemsDtoList;

        public Order ToDomain() => new()
        {
            CustomerName = CustomerName,
            OrderDate = OrderDate,
            OrderItems = OrderItemsDtoList.Select(i => i.ToDomain()).ToList(),
            Status = Order.OrderStatus.New
        };
    }
}