namespace MassTransit.Testing
{
    using System.Collections.Generic;
    using Context;
    using Microsoft.Extensions.Logging;
    using Registration;


    public class InMemoryTestHarnessRegistrationBusFactory :
        IRegistrationBusFactory
    {
        readonly string _virtualHost;

        public InMemoryTestHarnessRegistrationBusFactory(string virtualHost = null)
        {
            _virtualHost = virtualHost;
        }

        public IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications = null)
        {
            var inMemoryTestHarness = new InMemoryTestHarness(_virtualHost, specifications);

            inMemoryTestHarness.OnConfigureInMemoryBus += configurator =>
            {
                var loggerFactory = context.GetService<ILoggerFactory>();
                if (loggerFactory != null)
                    LogContext.ConfigureCurrentLogContext(loggerFactory);

                configurator.ConfigureEndpoints(context);
            };

            return new InMemoryTestHarnessBusInstance(inMemoryTestHarness, context);
        }
    }
}
