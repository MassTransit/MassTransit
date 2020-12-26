namespace MassTransit.Testing
{
    using System.Collections.Generic;
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

            inMemoryTestHarness.OnConfigureInMemoryBus += configurator => configurator.ConfigureEndpoints(context);

            return new InMemoryTestHarnessBusInstance(inMemoryTestHarness, context);
        }
    }
}
