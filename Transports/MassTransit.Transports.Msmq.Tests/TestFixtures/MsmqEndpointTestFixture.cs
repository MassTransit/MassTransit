namespace MassTransit.Transports.Msmq.TestFixtures
{
    using Configuration;
    using MassTransit.Tests.TextFixtures;

    public class MsmqEndpointTestFixture :
        EndpointTestFixture<MsmqEndpoint>
    {
        public IServiceBus LocalBus { get; private set; }
        public IServiceBus RemoteBus { get; private set; }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus = ServiceBusConfigurator.New(x => { x.ReceiveFrom("msmq://localhost/mt_client"); });

            RemoteBus = ServiceBusConfigurator.New(x => { x.ReceiveFrom("msmq://localhost/mt_server"); });
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