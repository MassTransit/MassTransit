namespace MassTransit.Testing
{
    using MessageObservers;
    using Saga;


    public interface ISagaInstance<out T> :
        IAsyncListElement
        where T : class, ISaga
    {
        T Saga { get; }
    }
}
