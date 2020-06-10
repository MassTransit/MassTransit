namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Microsoft.Extensions.Logging;


    public class RegistrationBusFactory :
        IRegistrationBusFactory
    {
        readonly Func<IBusRegistrationContext, IBusControl> _configure;

        public RegistrationBusFactory(Func<IBusRegistrationContext, IBusControl> configure)
        {
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

        public IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications)
        {
            var loggerFactory = context.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                LogContext.ConfigureCurrentLogContext(loggerFactory);

            var busControl = _configure(context);

            return new DefaultBusInstance(busControl);
        }
    }
}
