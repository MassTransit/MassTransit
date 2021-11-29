namespace MassTransit.Configuration
{
    public interface ICorrelationIdSelector<T>
        where T : class
    {
        bool TryGetSetCorrelationId(out IMessageCorrelationId<T> messageCorrelationId);
    }
}
