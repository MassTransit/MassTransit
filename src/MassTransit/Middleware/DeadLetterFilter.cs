namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Transports;


    /// <summary>
    /// If a message was neither delivered to a consumer nor caused a fault (which was notified already)
    /// then this filter will send the message to the dead letter pipe.
    /// </summary>
    public class DeadLetterFilter :
        IFilter<ReceiveContext>
    {
        readonly IPipe<ReceiveContext> _deadLetterPipe;

        public DeadLetterFilter(IPipe<ReceiveContext> deadLetterPipe)
        {
            _deadLetterPipe = deadLetterPipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("deadLetter");

            _deadLetterPipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ReceiveContext>.Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            await next.Send(context).ConfigureAwait(false);

            if (context.IsDelivered || context.IsFaulted)
                return;

            context.LogSkipped();

            await _deadLetterPipe.Send(context).ConfigureAwait(false);
        }
    }
}
