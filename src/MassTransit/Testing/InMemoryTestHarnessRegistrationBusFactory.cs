namespace MassTransit.Testing
{
    using System.Collections.Generic;
    using Registration;
    using Riders;


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
            var riders = new RiderConnectable();
            inMemoryTestHarness.OnConfigureInMemoryBus += configurator => configurator.ConfigureEndpoints(context);
            inMemoryTestHarness.OnConfigureBus += configurator => configurator.ConnectBusObserver(new RiderBusObserver(riders));
            return new InMemoryTestHarnessBusInstance(inMemoryTestHarness, riders);
        }
    }
}
