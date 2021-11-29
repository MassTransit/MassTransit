namespace MassTransit.Testing
{
    public interface ISagaInstance<out T> :
        IAsyncListElement
        where T : class, ISaga
    {
        T Saga { get; }
    }
}
