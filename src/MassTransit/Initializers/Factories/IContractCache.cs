namespace MassTransit.Initializers.Factories
{
    public interface IContractCache<T>
        where T : class
    {
        Contract[] Contracts { get; }
    }
}
