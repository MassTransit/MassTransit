namespace MassTransit.Pipeline.Filters
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    /// <summary>
    /// Performs the deserialization of a message ReceiveContext and passes the resulting
    /// ConsumeContext to the output pipe.
    /// </summary>
    public class DeserializeFilter :
        IFilter<ReceiveContext>
    {
        readonly IMessageDeserializer _deserializer;
        readonly IPipe<ConsumeContext> _output;

        public DeserializeFilter(IMessageDeserializer deserializer, IPipe<ConsumeContext> output)
        {
            _deserializer = deserializer;
            _output = output;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("deserialize");

            _deserializer.Probe(scope);
            _output.Probe(scope);
        }

        [DebuggerNonUserCode]
        public async Task Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            ConsumeContext consumeContext = _deserializer.Deserialize(context);

            if (context.TryGetPayload<StartedActivityContext>(out var activityContext))
                activityContext.AddConsumeContextHeaders(consumeContext);

            await _output.Send(consumeContext).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);

            await consumeContext.ConsumeCompleted.ConfigureAwait(false);
        }
    }
}
