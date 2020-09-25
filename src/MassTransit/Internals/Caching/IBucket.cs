namespace MassTransit.Internals.Caching
{
    using System.Threading.Tasks;


    public interface IBucket<TValue, in TCacheValue>
        where TValue : class
        where TCacheValue : ICacheValue<TValue>
    {
        Task Add(TCacheValue value);
    }
}
