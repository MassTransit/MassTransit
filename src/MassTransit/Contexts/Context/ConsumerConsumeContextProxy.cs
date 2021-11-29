namespace MassTransit.Context
{
    /// <summary>
    /// A consumer instance merged with a message consume context
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumerConsumeContextProxy<TConsumer, TMessage> :
        ConsumeContextProxy<TMessage>,
        ConsumerConsumeContext<TConsumer, TMessage>
        where TMessage : class
        where TConsumer : class
    {
        public ConsumerConsumeContextProxy(ConsumeContext<TMessage> context, TConsumer consumer)
            : base(context)
        {
            Consumer = consumer;
        }

        public TConsumer Consumer { get; }
    }
}
