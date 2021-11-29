namespace MassTransit.Context
{
    /// <summary>
    /// A consumer instance merged with a message consume context
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumerConsumeContextScope<TConsumer, TMessage> :
        ConsumeContextScope<TMessage>,
        ConsumerConsumeContext<TConsumer, TMessage>
        where TMessage : class
        where TConsumer : class
    {
        public ConsumerConsumeContextScope(ConsumeContext<TMessage> context, TConsumer consumer)
            : base(context)
        {
            Consumer = consumer;
        }

        public ConsumerConsumeContextScope(ConsumeContext<TMessage> context, TConsumer consumer, params object[] payloads)
            : base(context, payloads)
        {
            Consumer = consumer;
        }

        public TConsumer Consumer { get; }
    }
}
