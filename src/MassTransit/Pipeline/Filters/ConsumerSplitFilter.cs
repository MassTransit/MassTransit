namespace MassTransit.Pipeline.Filters
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using Pipes;


    /// <summary>
    /// Splits a context item off the pipe and carries it out-of-band to be merged
    /// once the next filter has completed
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumerSplitFilter<TConsumer, TMessage> :
        IFilter<ConsumerConsumeContext<TConsumer, TMessage>>
        where TMessage : class
        where TConsumer : class
    {
        readonly IFilter<ConsumerConsumeContext<TConsumer>> _next;

        public ConsumerSplitFilter(IFilter<ConsumerConsumeContext<TConsumer>> next)
        {
            _next = next;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("split");
            scope.Set(new {ConsumerType = TypeMetadataCache<TConsumer>.ShortName});

            _next.Probe(scope);
        }

        [DebuggerNonUserCode]
        public Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
        {
            var mergePipe = new ConsumerMergePipe<TConsumer, TMessage>(next);

            return _next.Send(context, mergePipe);
        }
    }
}
