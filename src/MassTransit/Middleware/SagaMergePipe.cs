namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;


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

        public Task Send(SagaConsumeContext<TSaga> context)
        {
            if (context is SagaConsumeContext<TSaga, TMessage> consumerContext)
                return _output.Send(consumerContext);

            throw new ArgumentException($"The message could not be retrieved: {TypeCache<TMessage>.ShortName}", nameof(context));
        }
    }
}
