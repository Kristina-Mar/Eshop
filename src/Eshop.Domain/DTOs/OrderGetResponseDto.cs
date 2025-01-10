namespace Eshop.Domain.DTOs
{
    public class OrderGetResponseDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderLineGetResponseDto> OrderItems { get; set; }
        public Order.OrderStatus Status { get; set; }

        public static OrderGetResponseDto FromDomain(Order order)
        {
            return new OrderGetResponseDto()
            {
                OrderId = order.OrderId,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(OrderLineGetResponseDto.FromDomain).ToList(),
                Status = order.Status
            };
        }
    }
}