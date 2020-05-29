namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public abstract class BasePerformanceCounters
    {
        readonly Lazy<CounterData[]> _counterCreationData;

        protected BasePerformanceCounters()
        {
            _counterCreationData = new Lazy<CounterData[]>(() => GetCounterData().ToArray());
            Initialize();
        }

        protected CounterData[] Data => _counterCreationData.Value;

        void Initialize()
        {
        }

        protected abstract IEnumerable<CounterData> GetCounterData();

        protected CounterData Convert(Counter counter, CounterType type)
        {
            return new CounterData
            {
                CounterName = counter.Name,
                CounterDescription = counter.Help,
                CounterType = type
            };
        }
    }
}
