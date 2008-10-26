namespace MassTransit.ServiceBus.Tests.TestConsumers
{
    using System;

    public class TestSelectiveConsumer<TMessage> :
        TestConsumerBase<TMessage>,
        Consumes<TMessage>.Selected
        where TMessage : class
    {
        private readonly Predicate<TMessage> _accept;

        public TestSelectiveConsumer(Predicate<TMessage> accept)
        {
            _accept = accept;
        }

        public TestSelectiveConsumer()
        {
            _accept = x => true;
        }

        public bool Accept(TMessage message)
        {
            return _accept(message);
        }
    }
}