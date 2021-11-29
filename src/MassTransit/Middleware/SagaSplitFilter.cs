namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Splits a context item off the pipe and carries it out-of-band to be merged
    /// once the next filter has completed
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SagaSplitFilter<TSaga, TMessage> :
        IFilter<SagaConsumeContext<TSaga, TMessage>>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly IFilter<SagaConsumeContext<TSaga>> _next;

        public SagaSplitFilter(IFilter<SagaConsumeContext<TSaga>> next)
        {
            _next = next;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("split");
            scope.Set(new { SagaType = TypeCache<TSaga>.ShortName });

            _next.Probe(scope);
        }

        public Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            var mergePipe = new SagaMergePipe<TSaga, TMessage>(next);

            return _next.Send(context, mergePipe);
        }
    }
}
