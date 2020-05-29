namespace MassTransit.Tests.Conventional
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Metadata;
    using Util;


    /// <summary>
    /// Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class CustomMethodConsumerMessageFilter<TConsumer, TMessage> :
        IConsumerMessageFilter<TConsumer, TMessage>
        where TConsumer : class, IHandler<TMessage>
        where TMessage : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("consume");
            scope.Add("method", $"Handle({TypeMetadataCache<TMessage>.ShortName} message)");
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumerConsumeContext<TConsumer, TMessage>>.Send(ConsumerConsumeContext<TConsumer, TMessage> context,
            IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
        {
            var messageConsumer = context.Consumer as IHandler<TMessage>;
            if (messageConsumer == null)
            {
                var message =
                    $"Consumer type {TypeMetadataCache<TConsumer>.ShortName} is not a consumer of message type {TypeMetadataCache<TMessage>.ShortName}";

                throw new ConsumerMessageException(message);
            }

            messageConsumer.Handle(context.Message);

            return TaskUtil.Completed;
        }
    }
}
