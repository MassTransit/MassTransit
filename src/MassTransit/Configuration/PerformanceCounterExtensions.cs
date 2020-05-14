namespace MassTransit
{
    using System;
    using Monitoring.Performance;
    using Monitoring.Performance.StatsD;


    public static class PerformanceCounterExtensions
    {
        public static void EnableStatsdPerformanceCounters(this IBusFactoryConfigurator configurator, Action<StatsDConfiguration> action)
        {
            var statsDConfiguration = StatsDConfiguration.Defaults();
            action(statsDConfiguration);

            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new PerformanceCounterBusObserver(new StatsDCounterFactory(statsDConfiguration));
            configurator.ConnectBusObserver(observer);
        }
    }
}
