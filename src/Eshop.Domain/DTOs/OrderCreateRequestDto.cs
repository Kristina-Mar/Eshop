namespace Eshop.Domain.DTOs
{
    public class OrderCreateRequestDto(string CustomerName, DateTime OrderDate, List<OrderItemCreateRequestDto> OrderItemsDtoList)
    {
        public string CustomerName { get; set; } = CustomerName;
        public DateTime OrderDate { get; set; } = OrderDate;
        public List<OrderItemCreateRequestDto> OrderItemsDtoList { get; set; } = OrderItemsDtoList;

        public Order ToDomain() => new()
        {
            CustomerName = CustomerName,
            OrderDate = OrderDate,
            OrderItems = OrderItemsDtoList.Select(i => i.ToDomain()).ToList(),
            Status = Order.OrderStatus.New
        };
    }
}