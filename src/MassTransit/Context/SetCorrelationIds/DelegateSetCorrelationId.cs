namespace MassTransit.Context.SetCorrelationIds
{
    using System;


    public class DelegateSetCorrelationId<T> :
        ISetCorrelationId<T>
        where T : class
    {
        readonly Func<T, Guid> _getCorrelationId;

        public DelegateSetCorrelationId(Func<T, Guid> getCorrelationId)
        {
            _getCorrelationId = getCorrelationId;
        }

        public void SetCorrelationId(SendContext<T> context)
        {
            var correlationId = _getCorrelationId(context.Message);
            context.CorrelationId = correlationId;
        }
    }
}
