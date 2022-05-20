namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Transports;


    public abstract class TransportRegistrationBusFactory<TEndpointConfigurator> :
        IRegistrationBusFactory
        where TEndpointConfigurator : class, IReceiveEndpointConfigurator
    {
        readonly IHostConfiguration _hostConfiguration;

        protected TransportRegistrationBusFactory(IHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
        }

        public abstract IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName);

        protected IBusInstance CreateBus<T, TConfigurator>(T configurator, IBusRegistrationContext context,
            Action<IBusRegistrationContext, TConfigurator> configure, IEnumerable<IBusInstanceSpecification> specifications)
            where T : TConfigurator, IBusFactory
            where TConfigurator : IBusFactoryConfigurator
        {
            LogContext.ConfigureCurrentLogContextIfNull(context);

            _hostConfiguration.LogContext = LogContext.Current;

            ConnectBusObservers(context, configurator);

            configure?.Invoke(context, configurator);

            IBusInstanceSpecification[] busInstanceSpecifications = specifications?.ToArray() ?? Array.Empty<IBusInstanceSpecification>();

            IEnumerable<ValidationResult> validationResult = configurator.Validate()
                .Concat(busInstanceSpecifications.SelectMany(x => x.Validate()));

            IReadOnlyList<ValidationResult> result = validationResult.ThrowIfContainsFailure("The bus configuration is invalid:");

            try
            {
                var busReceiveEndpointConfiguration = configurator.CreateBusEndpointConfiguration(x =>
                {
                    x.ConfigureConsumeTopology = false;

                    x.DiscardFaultedMessages();
                    x.DiscardSkippedMessages();
                });

                var host = _hostConfiguration.Build() as IHost<TEndpointConfigurator>;

                var bus = new MassTransitBus(host, _hostConfiguration.BusConfiguration.BusObservers, busReceiveEndpointConfiguration);

                ConnectReceiveEndpointObservers(context, bus);
                ConnectReceiveObservers(context, bus);
                ConnectConsumeObservers(context, bus);
                ConnectSendObservers(context, bus);
                ConnectPublishObservers(context, bus);

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

        static void ConnectBusObservers(IServiceProvider context, IBusObserverConnector connector)
        {
            foreach (var observer in context.GetServices<IBusObserver>())
                connector.ConnectBusObserver(observer);
        }

        static void ConnectReceiveEndpointObservers(IServiceProvider context, IReceiveEndpointObserverConnector connector)
        {
            foreach (var observer in context.GetServices<IReceiveEndpointObserver>())
                connector.ConnectReceiveEndpointObserver(observer);
        }

        static void ConnectReceiveObservers(IServiceProvider context, IReceiveObserverConnector connector)
        {
            foreach (var observer in context.GetServices<IReceiveObserver>())
                connector.ConnectReceiveObserver(observer);
        }

        static void ConnectConsumeObservers(IServiceProvider context, IConsumeObserverConnector connector)
        {
            foreach (var observer in context.GetServices<IConsumeObserver>())
                connector.ConnectConsumeObserver(observer);
        }

        static void ConnectSendObservers(IServiceProvider context, ISendObserverConnector connector)
        {
            foreach (var observer in context.GetServices<ISendObserver>())
                connector.ConnectSendObserver(observer);
        }

        static void ConnectPublishObservers(IServiceProvider context, IPublishObserverConnector connector)
        {
            foreach (var observer in context.GetServices<IPublishObserver>())
                connector.ConnectPublishObserver(observer);
        }

        protected virtual IBusInstance CreateBusInstance(IBusControl bus, IHost<TEndpointConfigurator> host, IHostConfiguration hostConfiguration,
            IBusRegistrationContext context)
        {
            return new TransportBusInstance<TEndpointConfigurator>(bus, host, hostConfiguration, context);
        }
    }
}
