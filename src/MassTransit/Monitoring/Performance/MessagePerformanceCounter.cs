namespace MassTransit.Monitoring.Performance
{
    using System;
    using Metadata;


    public class MessagePerformanceCounter<TMessage> :
        IDisposable,
        IMessagePerformanceCounter
    {
        readonly IPerformanceCounter _consumedPerSecond;
        readonly IPerformanceCounter _consumeDuration;
        readonly IPerformanceCounter _consumeDurationBase;
        readonly IPerformanceCounter _faulted;
        readonly IPerformanceCounter _faultPercentage;
        readonly IPerformanceCounter _faultPercentageBase;
        readonly IPerformanceCounter _publishedPerSecond;
        readonly IPerformanceCounter _publishFaulted;
        readonly IPerformanceCounter _publishFaultPercentage;
        readonly IPerformanceCounter _publishFaultPercentageBase;
        readonly IPerformanceCounter _sendFaulted;
        readonly IPerformanceCounter _sendFaultPercentage;
        readonly IPerformanceCounter _sendFaultPercentageBase;
        readonly IPerformanceCounter _sentPerSecond;
        readonly IPerformanceCounter _totalConsumed;
        readonly IPerformanceCounter _totalPublished;
        readonly IPerformanceCounter _totalSent;

        public MessagePerformanceCounter(ICounterFactory factory)
        {
            var messageType = TypeCache<TMessage>.ShortName;
            if (messageType.Length > 127)
                messageType = messageType.Substring(messageType.Length - 127);

            _totalConsumed = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.TotalReceived.CounterName, messageType);
            _consumedPerSecond = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumedPerSecond.CounterName, messageType);
            _consumeDuration = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumeDuration.CounterName, messageType);
            _consumeDurationBase = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumeDurationBase.CounterName, messageType);
            _faulted = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumeFaulted.CounterName, messageType);
            _faultPercentage = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumeFaultPercentage.CounterName, messageType);
            _faultPercentageBase = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.ConsumeFaultPercentageBase.CounterName, messageType);
            _totalSent = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.TotalSent.CounterName, messageType);
            _sentPerSecond = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.SentPerSecond.CounterName, messageType);
            _sendFaulted = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.SendFaulted.CounterName, messageType);
            _sendFaultPercentage = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.SendFaultPercentage.CounterName, messageType);
            _sendFaultPercentageBase = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.SendFaultPercentageBase.CounterName, messageType);
            _totalPublished = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.TotalPublished.CounterName, messageType);
            _publishedPerSecond = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.PublishedPerSecond.CounterName, messageType);
            _publishFaulted = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.PublishFaulted.CounterName, messageType);
            _publishFaultPercentage = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.PublishFaultPercentage.CounterName, messageType);
            _publishFaultPercentageBase = factory.Create(BuiltInCounters.Messages.Category,
                MessagePerformanceCounters.PublishFaultPercentageBase.CounterName, messageType);
        }

        public void Dispose()
        {
            _consumeDuration.Dispose();
            _consumeDurationBase.Dispose();
            _consumedPerSecond.Dispose();
            _faultPercentage.Dispose();
            _faultPercentageBase.Dispose();
            _faulted.Dispose();
            _totalConsumed.Dispose();
            _totalSent.Dispose();
            _sentPerSecond.Dispose();
            _sendFaulted.Dispose();
            _sendFaultPercentage.Dispose();
            _sendFaultPercentageBase.Dispose();
            _totalPublished.Dispose();
            _publishedPerSecond.Dispose();
            _publishFaulted.Dispose();
            _publishFaultPercentage.Dispose();
            _publishFaultPercentageBase.Dispose();
        }

        public void Consumed(TimeSpan duration)
        {
            _totalConsumed.Increment();
            _consumedPerSecond.Increment();

            _consumeDuration.IncrementBy((long)duration.TotalMilliseconds);
            _consumeDurationBase.Increment();

            _faultPercentageBase.Increment();
        }

        public void ConsumeFaulted(TimeSpan duration)
        {
            _totalConsumed.Increment();
            _consumedPerSecond.Increment();

            _faulted.Increment();

            _faultPercentage.Increment();
            _faultPercentageBase.Increment();
        }

        public void Sent()
        {
            _totalSent.Increment();
            _sentPerSecond.Increment();

            _sendFaultPercentageBase.Increment();
        }

        public void Published()
        {
            _totalPublished.Increment();
            _publishedPerSecond.Increment();

            _publishFaultPercentageBase.Increment();
        }

        public void PublishFaulted()
        {
            _totalPublished.Increment();
            _publishedPerSecond.Increment();

            _publishFaulted.Increment();

            _publishFaultPercentage.Increment();
            _publishFaultPercentageBase.Increment();
        }

        public void SendFaulted()
        {
            _totalSent.Increment();
            _sentPerSecond.Increment();

            _sendFaulted.Increment();

            _sendFaultPercentage.Increment();
            _sendFaultPercentageBase.Increment();
        }
    }
}
