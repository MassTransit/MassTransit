namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Context;


    /// <summary>
    /// Extracts the CorrelationId from the message where there is a one-to-one correlation
    /// identifier in the message (such as CorrelationId) and sets it in the header for use
    /// by the saga repository.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class CorrelationIdMessageFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly Func<ConsumeContext<TMessage>, Guid> _getCorrelationId;

        public CorrelationIdMessageFilter(Func<ConsumeContext<TMessage>, Guid> getCorrelationId)
        {
            if (getCorrelationId == null)
                throw new ArgumentNullException(nameof(getCorrelationId));

            _getCorrelationId = getCorrelationId;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("correlationId");
        }

        public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            var correlationId = _getCorrelationId(context);

            var proxy = new CorrelationIdConsumeContextProxy<TMessage>(context, correlationId);

            return next.Send(proxy);
        }
    }
}
