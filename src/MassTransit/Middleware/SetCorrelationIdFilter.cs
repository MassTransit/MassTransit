namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Context;


    /// <summary>
    /// Sets the CorrelationId header uses the supplied implementation.
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class SetCorrelationIdFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        readonly IMessageCorrelationId<T> _messageCorrelationId;

        public SetCorrelationIdFilter(IMessageCorrelationId<T> messageCorrelationId)
        {
            _messageCorrelationId = messageCorrelationId;
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            if (_messageCorrelationId.TryGetCorrelationId(context.Message, out var correlationId))
                context.CorrelationId = correlationId;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("SetCorrelationId");
        }
    }
}
