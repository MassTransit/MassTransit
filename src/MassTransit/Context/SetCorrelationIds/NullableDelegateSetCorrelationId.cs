namespace MassTransit.Context.SetCorrelationIds
{
    using System;


    public class NullableDelegateSetCorrelationId<T> :
        ISetCorrelationId<T>
        where T : class
    {
        readonly Func<T, Guid?> _getCorrelationId;

        public NullableDelegateSetCorrelationId(Func<T, Guid?> getCorrelationId)
        {
            _getCorrelationId = getCorrelationId;
        }

        public void SetCorrelationId(SendContext<T> context)
        {
            Guid? correlationId = _getCorrelationId(context.Message);
            if (correlationId.HasValue && correlationId.Value != Guid.Empty)
                context.CorrelationId = correlationId;
        }
    }
}
