namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Context;
    using Microsoft.Extensions.Logging;
    using Riders;


    public abstract class TransportRegistrationBusFactory :
        IRegistrationBusFactory
    {
        readonly IHostConfiguration _hostConfiguration;

        protected TransportRegistrationBusFactory(IHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
        }

        public abstract IBusInstance CreateBus(IRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications);

        protected IBusInstance CreateBus<T, TConfigurator>(T configurator, IRegistrationContext context,
            Action<IRegistrationContext, TConfigurator> configure, IEnumerable<IBusInstanceSpecification> specifications)
            where T : TConfigurator, IBusFactory
            where TConfigurator : IBusFactoryConfigurator
        {
            var loggerFactory = context.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                LogContext.ConfigureCurrentLogContext(loggerFactory);

            context.UseHealthCheck(configurator);

            var riders = new RiderConnectable();
            configurator.ConnectBusObserver(new RiderBusObserver(riders));

            configure?.Invoke(context, configurator);

            specifications ??= Enumerable.Empty<IBusInstanceSpecification>();

            var busControl = configurator.Build(specifications);

            var instance = new TransportBusInstance(busControl, _hostConfiguration, riders);

            foreach (var specification in specifications)
                specification.Configure(instance);

            return instance;
        }
    }
}
