namespace MassTransit.Topology.Filters
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    /// <summary>
    /// Sets the CorrelationId header uses the supplied implementation.
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class SetCorrelationIdFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        readonly ISetCorrelationId<T> _setCorrelationId;

        public SetCorrelationIdFilter(ISetCorrelationId<T> setCorrelationId)
        {
            _setCorrelationId = setCorrelationId;
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            _setCorrelationId.SetCorrelationId(context);

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("SetCorrelationId");
        }
    }
}
