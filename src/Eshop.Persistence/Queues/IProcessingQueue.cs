namespace Eshop.Persistence;

public interface IProcessingQueue<T> where T : class
{
    public void Add(T request);
    public T? Fetch();
}
