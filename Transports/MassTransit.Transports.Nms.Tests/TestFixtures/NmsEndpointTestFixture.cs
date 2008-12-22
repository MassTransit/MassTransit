namespace MassTransit.Transports.Nms.Tests.TestFixtures
{
    using Configuration;
    using MassTransit.Tests.TextFixtures;

    public class NmsEndpointTestFixture :
        EndpointTestFixture<NmsEndpoint>
    {
        public IServiceBus LocalBus { get; private set; }
        public IServiceBus RemoteBus { get; private set; }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus = ServiceBusConfigurator.New(x => { x.ReceiveFrom("activemq://localhost:61616/mt_client"); });

            RemoteBus = ServiceBusConfigurator.New(x => { x.ReceiveFrom("activemq://localhost:61616/mt_server"); });
        }

        protected override void TeardownContext()
        {
            LocalBus.Dispose();
            LocalBus = null;

            RemoteBus.Dispose();
            RemoteBus = null;

            base.TeardownContext();
        }
    }
}