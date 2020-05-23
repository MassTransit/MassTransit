namespace MassTransit.Registration
{
    using System;
    using Configuration;
    using Context;
    using Microsoft.Extensions.Logging;


    public abstract class TransportRegistrationBusFactory<TContainerContext> :
        IRegistrationBusFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly IHostConfiguration _hostConfiguration;

        protected TransportRegistrationBusFactory(IHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
        }

        public abstract IBusInstance CreateBus(IRegistrationContext<TContainerContext> context);

        protected IBusInstance CreateBus<T, TConfigurator>(T configurator, IRegistrationContext<TContainerContext> context,
            Action<IRegistrationContext<TContainerContext>, TConfigurator> configure)
            where T : TConfigurator, IBusFactory
            where TConfigurator : IBusFactoryConfigurator
        {
            var loggerFactory = context.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                LogContext.ConfigureCurrentLogContext(loggerFactory);

            context.UseHealthCheck(configurator);

            configure?.Invoke(context, configurator);

            var busControl = configurator.Build();

            return new TransportBusInstance(busControl, _hostConfiguration);
        }
    }
}
