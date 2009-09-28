namespace MassTransit.Tests.TestConsumers
{
    using System;

    public class TestReplyService<TMessage, TKey, TReplyMessage> :
        TestConsumerBase<TMessage>,
        Consumes<TMessage>.All
        where TMessage : class, CorrelatedBy<TKey>
        where TReplyMessage : class, CorrelatedBy<TKey>
    {
        public IServiceBus Bus { get; set; }

        public override void Consume(TMessage message)
        {
            base.Consume(message);

            var reply = (TReplyMessage) Activator.CreateInstance(typeof (TReplyMessage), message.CorrelationId);
            Bus.Publish(reply);
        }
    }
}