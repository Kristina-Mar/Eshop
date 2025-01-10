using Eshop.Domain;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Persistence.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly OrdersContext context;
        public OrderRepository(OrdersContext context)
        {
            this.context = context;
        }

        public IEnumerable<Order> Read()
        {
            return context.Orders.Include(o => o.OrderItems).ToList();
        }

        public Order ReadById(int orderId)
        {
            return context.Orders.Include(o => o.OrderItems).ToList().Find(o => o.OrderId == orderId);
        }

        public void Create(Order order)
        {
            context.Add(order);
            context.SaveChanges();
        }

        public void Update(int orderId, bool isPaid)
        {
            Order orderToUpdate = context.Orders.Find(orderId) ?? throw new KeyNotFoundException();
            if (isPaid)
            {
                context.Entry(orderToUpdate).CurrentValues.SetValues(orderToUpdate.Status = Order.OrderStatus.Paid);
            }
            else
            {
                context.Entry(orderToUpdate).CurrentValues.SetValues(orderToUpdate.Status = Order.OrderStatus.Cancelled);
            }
            context.SaveChanges();
        }
    }
}