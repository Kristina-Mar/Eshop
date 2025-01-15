using System.Collections.Concurrent;
using Eshop.Domain;

namespace Eshop.Persistence;

public class PaymentProcessingQueue() : IProcessingQueue<Order>
{
    private readonly ConcurrentQueue<Order> _queue = new();

    public void Add(Order request)
    {
        _queue.Enqueue(request);
    }

    public Order? Fetch()
    {
        if (_queue.TryDequeue(out Order request) is false)
        {
            return null;
        }
        return request;
    }
}
