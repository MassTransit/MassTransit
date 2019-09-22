namespace MassTransit.Internals.Reflection
{
    public interface IContractCache<T>
        where T : class
    {
        Contract[] Contracts { get; }
    }
}
