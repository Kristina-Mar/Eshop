using Eshop.Domain;

namespace Eshop.Persistence.Repositories
{
    public interface IRepository<T> where T : class
    {
        public IEnumerable<T> Read();
        public T ReadById(int orderId);
        public void Create(T order);
        public void Update(int orderId, bool isPaid);
    }
}