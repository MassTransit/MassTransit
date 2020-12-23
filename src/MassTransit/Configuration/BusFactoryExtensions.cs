namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Configurators;
    using Context;
    using GreenPipes;
    using Util;


    public static class BusFactoryExtensions
    {
        public static IBusControl Build(this IBusFactory factory, IBusConfiguration busConfiguration, IEnumerable<ISpecification> dependencies)
        {
            return Build(factory, busConfiguration, factory.Validate()
                .Concat(dependencies.SelectMany(x => x.Validate())));
        }

        public static IBusControl Build(this IBusFactory factory, IBusConfiguration busConfiguration)
        {
            return Build(factory, busConfiguration, factory.Validate());
        }

        static IBusControl Build(IBusFactory factory, IBusConfiguration busConfiguration, IEnumerable<ValidationResult> validationResult)
        {
            if (LogContext.Current == null)
                LogContext.ConfigureCurrentLogContext();

            busConfiguration.HostConfiguration.LogContext = LogContext.Current;

            var result = BusConfigurationResult.CompileResults(validationResult);

            try
            {
                var busReceiveEndpointConfiguration = factory.CreateBusEndpointConfiguration(x => x.ConfigureConsumeTopology = false);

                var host = busConfiguration.HostConfiguration.Build();

                var bus = new MassTransitBus(host, busConfiguration.BusObservers, busReceiveEndpointConfiguration);

                TaskUtil.Await(() => busConfiguration.BusObservers.PostCreate(bus));

                return bus;
            }
            catch (Exception ex)
            {
                TaskUtil.Await(() => busConfiguration.BusObservers.CreateFaulted(ex));

                throw new ConfigurationException(result, "An exception occurred during bus creation", ex);
            }
        }
    }
}
