namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Configurators;
    using Context;
    using GreenPipes;
    using Microsoft.Extensions.Logging;


    public abstract class TransportRegistrationBusFactory<TEndpointConfigurator> :
        IRegistrationBusFactory
        where TEndpointConfigurator : class, IReceiveEndpointConfigurator
    {
        readonly IHostConfiguration _hostConfiguration;

        protected TransportRegistrationBusFactory(IHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
        }

        public abstract IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications);

        protected IBusInstance CreateBus<T, TConfigurator>(T configurator, IBusRegistrationContext context,
            Action<IBusRegistrationContext, TConfigurator> configure, IEnumerable<IBusInstanceSpecification> specifications)
            where T : TConfigurator, IBusFactory
            where TConfigurator : IBusFactoryConfigurator
        {
            var loggerFactory = context.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                LogContext.ConfigureCurrentLogContext(loggerFactory);
            else if (LogContext.Current == null)
                LogContext.ConfigureCurrentLogContext();

            _hostConfiguration.LogContext = LogContext.Current;

            configure?.Invoke(context, configurator);

            specifications ??= Enumerable.Empty<IBusInstanceSpecification>();

            IEnumerable<IBusInstanceSpecification> busInstanceSpecifications = specifications as IBusInstanceSpecification[] ?? specifications.ToArray();

            IEnumerable<ValidationResult> validationResult = configurator.Validate()
                .Concat(busInstanceSpecifications.SelectMany(x => x.Validate()));

            var result = BusConfigurationResult.CompileResults(validationResult);

            try
            {
                var busReceiveEndpointConfiguration = configurator.CreateBusEndpointConfiguration(x => x.ConfigureConsumeTopology = false);

                var host = _hostConfiguration.Build() as IHost<TEndpointConfigurator>;

                var bus = new MassTransitBus(host, _hostConfiguration.BusConfiguration.BusObservers, busReceiveEndpointConfiguration);

                _hostConfiguration.BusConfiguration.BusObservers.PostCreate(bus);

                var instance = CreateBusInstance(bus, host, _hostConfiguration, context);

                foreach (var specification in busInstanceSpecifications)
                    specification.Configure(instance);

                return instance;
            }
            catch (Exception ex)
            {
                _hostConfiguration.BusConfiguration.BusObservers.CreateFaulted(ex);

                throw new ConfigurationException(result, "An exception occurred during bus creation", ex);
            }
        }

        protected virtual IBusInstance CreateBusInstance(IBusControl bus, IHost<TEndpointConfigurator> host, IHostConfiguration hostConfiguration,
            IBusRegistrationContext context)
        {
            return new TransportBusInstance<TEndpointConfigurator>(bus, host, hostConfiguration, context);
        }
    }
}
