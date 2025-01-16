namespace Eshop.Persistence.Repositories
{
    public interface IRepositoryAsync<T> where T : class
    {
        public Task<IEnumerable<T>> ReadAsync();
        public Task<T>? ReadByIdAsync(int orderId);
        public Task CreateAsync(T order);
        public Task UpdateAsync(T order);
    }
}