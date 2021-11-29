namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Context;


    /// <summary>
    /// Merges the out-of-band Saga back into the context
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SagaMessageMergePipe<TSaga, TMessage> :
        IPipe<ConsumeContext<TMessage>>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly SagaConsumeContext<TSaga, TMessage> _context;
        readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _output;

        public SagaMessageMergePipe(IPipe<SagaConsumeContext<TSaga, TMessage>> output, SagaConsumeContext<TSaga, TMessage> context)
        {
            _output = output;
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("merge");
            scope.Set(new
            {
                SagaType = TypeCache<TSaga>.ShortName,
                MessageType = TypeCache<TMessage>.ShortName
            });

            _output.Probe(scope);
        }

        public Task Send(ConsumeContext<TMessage> context)
        {
            if (ReferenceEquals(context, _context))
                return _output.Send(_context);

            return context is SagaConsumeContext<TSaga, TMessage> consumerContext
                ? _output.Send(consumerContext)
                : _output.Send(new SagaConsumeContextProxy<TSaga, TMessage>(context, _context));
        }
    }
}
