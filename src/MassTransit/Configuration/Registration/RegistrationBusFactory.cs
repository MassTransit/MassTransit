namespace MassTransit.Registration
{
    using System;
    using Context;
    using Microsoft.Extensions.Logging;


    public class RegistrationBusFactory<TContainerContext> :
        IRegistrationBusFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly Func<IRegistrationContext<TContainerContext>, IBusControl> _configure;

        public RegistrationBusFactory(Func<IRegistrationContext<TContainerContext>, IBusControl> configure)
        {
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

        public IBusInstance CreateBus(IRegistrationContext<TContainerContext> context)
        {
            var loggerFactory = context.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                LogContext.ConfigureCurrentLogContext(loggerFactory);

            var busControl = _configure(context);

            return new DefaultBusInstance(busControl);
        }
    }
}
