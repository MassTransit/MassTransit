namespace MassTransit.Topology.Conventions.CorrelationId
{
    using Context;


    public interface ICorrelationIdSelector<T>
        where T : class
    {
        bool TryGetSetCorrelationId(out IMessageCorrelationId<T> messageCorrelationId);
    }
}
