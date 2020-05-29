namespace MassTransit.Context.SetCorrelationIds
{
    public class NoSetCorrelationId<T> :
        ISetCorrelationId<T>
        where T : class
    {
        public void SetCorrelationId(SendContext<T> context)
        {
        }
    }
}
