namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Context;
    using Microsoft.Extensions.Logging;
    using Riders;


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


        class DefaultBusInstance :
            IBusInstance
        {
            const string RiderExceptionMessage =
                "Riders could be only used with Microsoft DI or Autofac using 'SetBusFactory' method (UsingTransport extensions).";

            public DefaultBusInstance(IBusControl busControl)
            {
                BusControl = busControl;
            }

            public Type InstanceType => typeof(IBus);
            public IBus Bus => BusControl;
            public IBusControl BusControl { get; }

            public IHostConfiguration HostConfiguration => default;

            public void Connect<TRider>(IRiderControl riderControl)
                where TRider : IRider
            {
                throw new ConfigurationException(RiderExceptionMessage);
            }

            public TRider GetRider<TRider>()
                where TRider : IRider
            {
                throw new ConfigurationException(RiderExceptionMessage);
            }
        }
    }
}
