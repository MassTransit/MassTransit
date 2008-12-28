namespace MassTransit.Tests.Services.HealthMonitoring
{
    using Magnum.Common.DateTimeExtensions;
    using MassTransit.Services.HealthMonitoring;
    using MassTransit.Services.HealthMonitoring.Messages;
    using MassTransit.Services.HealthMonitoring.Server;
    using NUnit.Framework;
    using Rhino.Mocks;
    using TextFixtures;

    [TestFixture]
    public class When_a_suspect_is_received :
        LoopbackLocalAndRemoteTestFixture
    {
        Investigator _investigator;

        private HealthInformation information;
        private IHealthCache _healthCache;
        
        protected override void EstablishContext()
        {
            base.EstablishContext();
            _healthCache = new LocalHealthCache();
            ObjectBuilder.Stub(x => x.GetInstance<IHealthCache>()).Return(_healthCache);

            information = new HealthInformation(RemoteBus.Endpoint.Uri, 3);
            _investigator = new Investigator(LocalBus, ObjectBuilder.GetInstance<IEndpointFactory>(), ObjectBuilder.GetInstance<IHealthCache>());
        }

        [Test]
        public void An_Investigation_should_happen()
        {
            FutureMessage<Ping> fm = new FutureMessage<Ping>();
            LocalBus.Subscribe(_investigator);

            RemoteBus.Subscribe<Ping>(msg => fm.Set(msg));

            _investigator.Consume(new Suspect(RemoteBus.Endpoint.Uri));

            fm.IsAvailable(1.Seconds())
                .ShouldBeTrue();
        }

        [Test]
        public void An_investigation_should_happen_and_pong_received()
        {
            FutureMessage<Ping> fmPing = new FutureMessage<Ping>();
            //FutureMessage<Pong> fmPong = new FutureMessage<Pong>();
            RemoteBus.Subscribe<Ping>(msg =>
                                          {
                                              RemoteBus.Publish(new Pong(msg.CorrelationId, RemoteBus.Endpoint.Uri));
                                              fmPing.Set(msg);
                                          }); 

            

            _investigator.Consume(new Suspect(RemoteBus.Endpoint.Uri));
            _investigator.Enabled
                .ShouldBeTrue();
            fmPing.IsAvailable(1.Seconds())
                .ShouldBeTrue();

            _investigator.Consume(new Pong(_investigator.CorrelationId, RemoteBus.Endpoint.Uri));
            _investigator.Enabled
                .ShouldBeFalse();
        }

        [Test]
        public void When_a_ping_times_out()
        {
            FutureMessage<Ping> fmPing = new FutureMessage<Ping>();
            FutureMessage<DownEndpoint> fmDown = new FutureMessage<DownEndpoint>();

            RemoteBus.Subscribe<DownEndpoint>((msg) => fmDown.Set(msg));
            _investigator = new Investigator(LocalBus, ObjectBuilder.GetInstance<IEndpointFactory>(),
                                             ObjectBuilder.GetInstance<IHealthCache>(), 1);
            _investigator.Consume(new Suspect(RemoteBus.Endpoint.Uri));
            fmDown.IsAvailable(2.Seconds());


        }
    }
}