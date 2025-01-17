using Eshop.Domain;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Persistence.Repositories
{
    public class OrderRepositoryAsync : IRepositoryAsync<Order>
    {
        private readonly OrdersContext context;
        public OrderRepositoryAsync(OrdersContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Order>> ReadAsync()
        {
            return await context.Orders.Include(o => o.OrderItems).ToListAsync();
        }

        public async Task<Order>? ReadByIdAsync(int orderId)
        {
            return await context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task CreateAsync(Order order)
        {
            await context.AddAsync(order);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order updatedOrder)
        {
            context.Orders.Update(updatedOrder);
            await context.SaveChangesAsync();
        }
    }
}