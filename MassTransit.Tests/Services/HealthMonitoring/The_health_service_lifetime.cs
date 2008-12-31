namespace MassTransit.Tests.Services.HealthMonitoring
{
    using System;
    using Magnum.Common.DateTimeExtensions;
    using MassTransit.Services.HealthMonitoring;
    using MassTransit.Services.HealthMonitoring.Messages;
    using MassTransit.Services.HealthMonitoring.Server;
    using NUnit.Framework;
    using Rhino.Mocks;
    using TextFixtures;

    [TestFixture]
    public class The_health_service_lifetime :
        LoopbackLocalAndRemoteTestFixture
    {
        private HealthService _healthService;
        private IHealthCache healthCache;
        private IHeartbeatTimer healthTimer;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            //LocalBus is the HealthService
            healthCache = new LocalHealthCache();
            healthTimer = new InMemoryHeartbeatTimer(LocalBus);
            ObjectBuilder.Stub(x => x.GetInstance<HeartbeatMonitor>()).Return(new HeartbeatMonitor(healthCache, healthTimer));
            ObjectBuilder.Stub(x => x.GetInstance<Investigator>()).Return(new Investigator(LocalBus,ObjectBuilder.GetInstance<IEndpointFactory>(),healthCache));
            ObjectBuilder.Stub(x => x.GetInstance<Reporter>()).Return(new Reporter());
            _healthService = new HealthService(LocalBus);
            _healthService.Start();

            //RemoteBus is the HealthClient
        }

        protected override void TeardownContext()
        {
            _healthService.Stop();
            healthTimer = null;
            healthCache = null;

            base.TeardownContext();
        }

        [Test]
        public void If_I_stop_beating_I_get_pinged()
        {
            AppDomain.CurrentDomain.UnhandledException += Bob;
            Guid id = Guid.NewGuid();
            FutureMessage<Pong> fm = new FutureMessage<Pong>();
            RemoteBus.Subscribe<Pong>(msg=> { fm.Set(msg); });
            RemoteBus.Publish(new Heartbeat(0, RemoteBus.Endpoint.Uri));




            fm.IsAvailable(2.Seconds())
                .ShouldBeTrue();
        }

        private void Bob(object sender, UnhandledExceptionEventArgs e)
        {
            int i = 0;
            i++;
        }
    }
}