namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;


    /// <summary>
    /// Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class MethodConsumerMessageFilter<TConsumer, TMessage> :
        IConsumerMessageFilter<TConsumer, TMessage>
        where TConsumer : class, IConsumer<TMessage>
        where TMessage : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("consume");
            scope.Add("method", $"Consume(ConsumeContext<{TypeCache<TMessage>.ShortName}> context)");
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumerConsumeContext<TConsumer, TMessage>>.Send(ConsumerConsumeContext<TConsumer, TMessage> context,
            IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
        {
            if (context.Consumer is IConsumer<TMessage> messageConsumer)
                return messageConsumer.Consume(context);

            var message = $"Consumer type {TypeCache<TConsumer>.ShortName} is not a consumer of message type {TypeCache<TMessage>.ShortName}";

            throw new ConsumerMessageException(message);
        }
    }
}
