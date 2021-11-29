namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Testing;
    using Testing.Implementations;
    using Transports;


    public class InMemoryTestHarnessRegistrationBusFactory :
        IRegistrationBusFactory
    {
        readonly string _virtualHost;

        public InMemoryTestHarnessRegistrationBusFactory(string virtualHost = null)
        {
            _virtualHost = virtualHost;
        }

        public IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
        {
            var inMemoryTestHarness = new InMemoryTestHarness(_virtualHost, specifications);

            inMemoryTestHarness.OnConfigureInMemoryBus += configurator =>
            {
                var loggerFactory = context.GetService<ILoggerFactory>();
                if (loggerFactory != null)
                    LogContext.ConfigureCurrentLogContext(loggerFactory);
            };
            inMemoryTestHarness.OnInMemoryBusConfigured += configurator =>
            {
                configurator.ConfigureEndpoints(context);
            };

            return new InMemoryTestHarnessBusInstance(inMemoryTestHarness, context);
        }
    }
}
