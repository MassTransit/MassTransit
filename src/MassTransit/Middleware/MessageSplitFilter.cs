namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;


    /// <summary>
    /// Splits a context item off the pipe and carries it out-of-band to be merged
    /// once the next filter has completed
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class MessageSplitFilter<TConsumer, TMessage> :
        IFilter<ConsumerConsumeContext<TConsumer, TMessage>>
        where TMessage : class
        where TConsumer : class
    {
        readonly IFilter<ConsumeContext<TMessage>> _next;

        public MessageSplitFilter(IFilter<ConsumeContext<TMessage>> next)
        {
            _next = next;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("split");
            scope.Set(new {MessageType = TypeCache<TMessage>.ShortName});

            _next.Probe(scope);
        }

        [DebuggerNonUserCode]
        public Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
        {
            var mergePipe = new ConsumerMessageMergePipe<TConsumer, TMessage>(next, context);

            return _next.Send(context, mergePipe);
        }
    }
}
