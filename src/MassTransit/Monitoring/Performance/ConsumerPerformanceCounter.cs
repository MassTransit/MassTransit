namespace MassTransit.Monitoring.Performance
{
    using System;


    /// <summary>
    /// Tracks the consumption and failure of a consumer processing messages. The message types
    /// in this case are not included in the counter, only the consumer itself.
    /// </summary>
    public class ConsumerPerformanceCounter :
        IDisposable,
        IConsumerPerformanceCounter
    {
        readonly IPerformanceCounter _consumeRate;
        readonly IPerformanceCounter _duration;
        readonly IPerformanceCounter _durationBase;
        readonly IPerformanceCounter _faultPercentage;
        readonly IPerformanceCounter _faultPercentageBase;
        readonly IPerformanceCounter _totalFaults;
        readonly IPerformanceCounter _totalMessages;

        public ConsumerPerformanceCounter(ICounterFactory factory, string consumerType)
        {
            if (consumerType.Length > 127)
                consumerType = consumerType.Substring(consumerType.Length - 127);

            _totalMessages = factory.Create(BuiltInCounters.Consumers.Category, ConsumerPerformanceCounters.TotalMessages.CounterName, consumerType);
            _consumeRate = factory.Create(BuiltInCounters.Consumers.Category, ConsumerPerformanceCounters.ConsumeRate.CounterName, consumerType);
            _duration = factory.Create(BuiltInCounters.Consumers.Category, ConsumerPerformanceCounters.Duration.CounterName, consumerType);
            _durationBase = factory.Create(BuiltInCounters.Consumers.Category, ConsumerPerformanceCounters.DurationBase.CounterName, consumerType);
            _totalFaults = factory.Create(BuiltInCounters.Consumers.Category, ConsumerPerformanceCounters.TotalFaults.CounterName, consumerType);
            _faultPercentage = factory.Create(BuiltInCounters.Consumers.Category, ConsumerPerformanceCounters.FaultPercentage.CounterName, consumerType);
            _faultPercentageBase = factory.Create(BuiltInCounters.Consumers.Category, ConsumerPerformanceCounters.FaultPercentageBase.CounterName,
                consumerType);
        }

        public void Consumed(TimeSpan duration)
        {
            _totalMessages.Increment();
            _consumeRate.Increment();

            _duration.IncrementBy((long)duration.TotalMilliseconds);
            _durationBase.Increment();

            _faultPercentageBase.Increment();
        }

        public void Faulted()
        {
            _totalMessages.Increment();
            _consumeRate.Increment();

            _totalFaults.Increment();

            _faultPercentage.Increment();
            _faultPercentageBase.Increment();
        }

        public void Dispose()
        {
            _duration.Dispose();
            _durationBase.Dispose();
            _consumeRate.Dispose();
            _faultPercentage.Dispose();
            _faultPercentageBase.Dispose();
            _totalFaults.Dispose();
            _totalMessages.Dispose();
        }
    }
}
