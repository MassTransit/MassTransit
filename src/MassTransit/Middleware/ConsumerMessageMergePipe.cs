namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Context;


    /// <summary>
    /// Merges the out-of-band consumer back into the context
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumerMessageMergePipe<TConsumer, TMessage> :
        IPipe<ConsumeContext<TMessage>>
        where TMessage : class
        where TConsumer : class
    {
        readonly ConsumerConsumeContext<TConsumer, TMessage> _context;
        readonly IPipe<ConsumerConsumeContext<TConsumer, TMessage>> _output;

        public ConsumerMessageMergePipe(IPipe<ConsumerConsumeContext<TConsumer, TMessage>> output, ConsumerConsumeContext<TConsumer, TMessage> context)
        {
            _output = output;
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("merge");
            scope.Set(new
            {
                ConsumerType = TypeCache<TConsumer>.ShortName,
                MessageType = TypeCache<TMessage>.ShortName
            });

            _output.Probe(scope);
        }

        public Task Send(ConsumeContext<TMessage> context)
        {
            if (ReferenceEquals(context, _context))
                return _output.Send(_context);

            return context is ConsumerConsumeContext<TConsumer, TMessage> consumerContext
                ? _output.Send(consumerContext)
                : _output.Send(new ConsumerConsumeContextScope<TConsumer, TMessage>(context, _context.Consumer));
        }
    }
}
