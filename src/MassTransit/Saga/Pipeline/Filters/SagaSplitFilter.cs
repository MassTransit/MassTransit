namespace MassTransit.Saga.Pipeline.Filters
{
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Pipes;


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

        public Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            var mergePipe = new SagaMergePipe<TSaga, TMessage>(next);

            return _next.Send(context, mergePipe);
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this, x => _next.Visit(x) && _next.Visit(x));
        }
    }
}