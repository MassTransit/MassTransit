namespace MassTransit.Registration
{
    /// <summary>
    /// Bind is used to store types bound to their owner, such as an IBusControl to an IMyBus.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The bound type</typeparam>
    public class Bind<TKey, TValue>
        where TValue : class
    {
        public Bind(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }
    }


    public static class Bind<TKey>
    {
        public static Bind<TKey, TValue> Create<TValue>(TValue value)
            where TValue : class
        {
            return new Bind<TKey, TValue>(value);
        }
    }
}
