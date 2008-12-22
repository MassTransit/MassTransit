namespace MassTransit.Transports.Wcf.Tests.TestFixtures
{
    using Configuration;
    using MassTransit.Tests.TextFixtures;

    public class WcfEndpointTestFixture :
        EndpointTestFixture<WcfEndpoint>
    {
        public IServiceBus LocalBus { get; private set; }
        public IServiceBus RemoteBus { get; private set; }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus = ServiceBusConfigurator.New(x => { x.ReceiveFrom("net.tcp://localhost:8061/local_sb"); });

            RemoteBus = ServiceBusConfigurator.New(x => { x.ReceiveFrom("net.tcp://localhost:8061/remote_sb"); });
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