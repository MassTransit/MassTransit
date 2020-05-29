namespace MassTransit.Context
{
    public interface ISetCorrelationId<in T>
        where T : class
    {
        void SetCorrelationId(SendContext<T> context);
    }
}
