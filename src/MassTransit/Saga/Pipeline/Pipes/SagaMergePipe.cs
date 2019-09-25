namespace MassTransit.Saga.Pipeline.Pipes
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;


    /// <summary>
    /// Merges the out-of-band message back into the pipe
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public struct SagaMergePipe<TSaga, TMessage> :
        IPipe<SagaConsumeContext<TSaga>>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _output;

        public SagaMergePipe(IPipe<SagaConsumeContext<TSaga, TMessage>> output)
        {
            _output = output;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("merge");
            scope.Set(new
            {
                SagaType = TypeMetadataCache<TSaga>.ShortName,
                MessageType = TypeMetadataCache<TMessage>.ShortName
            });

            _output.Probe(scope);
        }

        public Task Send(SagaConsumeContext<TSaga> context)
        {
            if (context is SagaConsumeContext<TSaga, TMessage> consumerContext)
                return _output.Send(consumerContext);

            throw new ArgumentException($"THe message could not be retrieved: {TypeMetadataCache<TMessage>.ShortName}", nameof(context));
        }
    }
}
