namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Context;


    /// <summary>
    /// Merges the out-of-band consumer back into the pipe
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumerMergePipe<TConsumer, TMessage> :
        IPipe<ConsumerConsumeContext<TConsumer>>
        where TMessage : class
        where TConsumer : class
    {
        readonly IPipe<ConsumerConsumeContext<TConsumer, TMessage>> _output;

        public ConsumerMergePipe(IPipe<ConsumerConsumeContext<TConsumer, TMessage>> output)
        {
            _output = output;
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

        public Task Send(ConsumerConsumeContext<TConsumer> context)
        {
            if (context is ConsumerConsumeContext<TConsumer, TMessage> consumerContext)
                return _output.Send(consumerContext);

            if (context.TryGetMessage(out ConsumeContext<TMessage> messageContext))
                return _output.Send(new ConsumerConsumeContextScope<TConsumer, TMessage>(messageContext, context.Consumer));

            throw new ArgumentException($"THe message could not be retrieved: {TypeCache<TMessage>.ShortName}", nameof(context));
        }
    }
}
