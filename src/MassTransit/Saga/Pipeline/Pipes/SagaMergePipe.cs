namespace MassTransit.Saga.Pipeline.Pipes
{
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Monitoring.Introspection;


    /// <summary>
    /// Merges the out-of-band message back into the pipe
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SagaMergePipe<TSaga, TMessage> :
        IPipe<SagaConsumeContext<TSaga>>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _output;

        public SagaMergePipe(IPipe<SagaConsumeContext<TSaga, TMessage>> output)
        {
            _output = output;
        }

        Task IProbeSite.Probe(ProbeContext context)
        {
            return _output.Probe(context);
        }

        public Task Send(SagaConsumeContext<TSaga> context)
        {
            return _output.Send(context.PopContext<TMessage>());
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this, x => _output.Visit(x));
        }
    }
}