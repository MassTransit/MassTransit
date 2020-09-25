namespace MassTransit.Internals.Caching
{
    using System.Threading.Tasks;


    public interface IValueTracker<TValue, TCacheValue>
        where TValue : class
        where TCacheValue : ICacheValue<TValue>
    {
        int Capacity { get; }

        Task Add(TCacheValue value);

        Task ReBucket(IBucket<TValue, TCacheValue> source, TCacheValue value);
    }
}
