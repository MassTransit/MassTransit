namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Collections.Generic;


    public class ConsumerPerformanceCounters :
        BasePerformanceCounters
    {
        public static CounterData ConsumeRate => Cached.Instance.Value.Data[0];
        public static CounterData TotalMessages => Cached.Instance.Value.Data[1];
        public static CounterData Duration => Cached.Instance.Value.Data[2];
        public static CounterData DurationBase => Cached.Instance.Value.Data[3];
        public static CounterData TotalFaults => Cached.Instance.Value.Data[4];
        public static CounterData FaultPercentage => Cached.Instance.Value.Data[5];
        public static CounterData FaultPercentageBase => Cached.Instance.Value.Data[6];

        public static void Install()
        {
            _ = Cached.Instance.Value;
        }

        protected override IEnumerable<CounterData> GetCounterData()
        {
            yield return Convert(BuiltInCounters.Consumers.Counters.MessagesPerSecond, CounterType.RateOfCountsPerSecond32);
            yield return Convert(BuiltInCounters.Consumers.Counters.TotalMessages, CounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Consumers.Counters.AverageDuration, CounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Consumers.Counters.AverageDurationBase, CounterType.AverageBase);
            yield return Convert(BuiltInCounters.Consumers.Counters.TotalFaults, CounterType.NumberOfItems64);
            yield return Convert(BuiltInCounters.Consumers.Counters.FaultPercent, CounterType.AverageCount64);
            yield return Convert(BuiltInCounters.Consumers.Counters.FaultPercentBase, CounterType.AverageBase);
        }


        static class Cached
        {
            internal static readonly Lazy<ConsumerPerformanceCounters> Instance =
                new Lazy<ConsumerPerformanceCounters>(() => new ConsumerPerformanceCounters());
        }
    }
}
