namespace MassTransit.Tests.TestConsumers
{
    using System;

    public class TestCorrelatedConsumer<TMessage, TKey> :
        TestConsumerBase<TMessage>,
        Consumes<TMessage>.For<TKey>
        where TMessage : class, CorrelatedBy<Guid>
    {
        private readonly TKey _correlationId;

        public TestCorrelatedConsumer(TKey correlationId)
        {
            _correlationId = correlationId;
        }

        public TKey CorrelationId
        {
            get { return _correlationId; }
        }
    }
}