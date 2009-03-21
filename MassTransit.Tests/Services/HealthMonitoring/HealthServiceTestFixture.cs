namespace MassTransit.Tests.Services.HealthMonitoring
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Configuration;
    using MassTransit.Pipeline.Interceptors;
    using MassTransit.Saga;
    using MassTransit.Saga.Pipeline;
    using MassTransit.Services.HealthMonitoring;
    using MassTransit.Services.HealthMonitoring.Configuration;
    using MassTransit.Services.HealthMonitoring.Messages;
    using MassTransit.Services.HealthMonitoring.Server;
    using MassTransit.Services.Subscriptions.Client;
    using MassTransit.Services.Subscriptions.Configuration;
    using MassTransit.Services.Subscriptions.Messages;
    using MassTransit.Services.Subscriptions.Server;
    using MassTransit.Services.Timeout.Messages;
    using MassTransit.Subscriptions;
    using MassTransit.Transports;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using Rhino.Mocks;
    using TextFixtures;
    using ISubscriptionRepository=MassTransit.Services.Subscriptions.Server.ISubscriptionRepository;
    using SubscriptionClientSaga=MassTransit.Services.Subscriptions.Server.SubscriptionClientSaga;
    using SubscriptionSaga=MassTransit.Services.Subscriptions.Server.SubscriptionSaga;

    [TestFixture]
    public class HealthServiceTestFixture :
        EndpointTestFixture<LoopbackEndpoint>
    {
        private ISagaRepository<HealthSaga> _healthSagaRepository;
        private ISagaRepository<SubscriptionSaga> _subscriptionSagaRepository;
        public ISubscriptionRepository SubscriptionRepository { get; private set; }
        private ISagaRepository<SubscriptionClientSaga> _subscriptionClientSagaRepository;
        public IServiceBus SubscriptionBus { get; private set; }
        public MassTransit.Services.Subscriptions.Server.SubscriptionService SubscriptionService { get; private set; }
        public HealthService HealthService { get; private set; }
        public IServiceBus LocalBus { get; private set; }
        public IServiceBus RemoteBus { get; private set; }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            ServiceLocator.SetLocatorProvider(()=>ObjectBuilder);
            const string subscriptionServiceEndpointAddress = "loopback://localhost/mt_subscriptions";

            SubscriptionBus = ServiceBusConfigurator.New(x => { x.ReceiveFrom(subscriptionServiceEndpointAddress); });

            SetupSubscriptionService();

            LocalBus = ServiceBusConfigurator.New(x =>
            {
                x.ConfigureService<SubscriptionClientConfigurator>(y =>
                {
                    // setup endpoint
                    y.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointAddress);
                });
                x.ReceiveFrom("loopback://localhost/mt_client");
            });

            RemoteBus = ServiceBusConfigurator.New(x =>
            {

                x.ConfigureService<SubscriptionClientConfigurator>(y =>
                {
                    // setup endpoint
                    y.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointAddress);
                });
                x.ReceiveFrom("loopback://localhost/mt_server");
            });

            SetupHealthService();
        }

        private void SetupSubscriptionService()
        {
            //SubscriptionRepository = new InMemorySubscriptionRepository();
            SubscriptionRepository = MockRepository.GenerateMock<ISubscriptionRepository>();
            SubscriptionRepository.Expect(x => x.List()).Return(new List<Subscription>());
            ObjectBuilder.Stub(x => x.GetInstance<ISubscriptionRepository>())
                .Return(SubscriptionRepository);

            _subscriptionClientSagaRepository = SetupSagaRepository<SubscriptionClientSaga>();
            SetupInitiateSagaSink<SubscriptionClientSaga, CacheUpdateRequest>(SubscriptionBus, _subscriptionClientSagaRepository);
            SetupOrchestrateSagaSink<SubscriptionClientSaga, CancelSubscriptionUpdates>(SubscriptionBus, _subscriptionClientSagaRepository);

            _subscriptionSagaRepository = SetupSagaRepository<SubscriptionSaga>();
            SetupInitiateSagaSink<SubscriptionSaga, AddSubscription>(SubscriptionBus, _subscriptionSagaRepository);
            SetupOrchestrateSagaSink<SubscriptionSaga, RemoveSubscription>(SubscriptionBus, _subscriptionSagaRepository);

            SubscriptionService = new SubscriptionService(SubscriptionBus, SubscriptionRepository, EndpointFactory, _subscriptionSagaRepository, _subscriptionClientSagaRepository);

            SubscriptionService.Start();

            ObjectBuilder.Stub(x => x.GetInstance<SubscriptionClient>())
                .Return(null)
                .WhenCalled(invocation => { invocation.ReturnValue = new SubscriptionClient(EndpointFactory); });
        }

        private void SetupHealthService()
        {
            _healthSagaRepository = SetupSagaRepository<HealthSaga>();
            SetupInitiateSagaStateMachineSink<HealthSaga, EndpointTurningOn>(RemoteBus, _healthSagaRepository);
            SetupOrchestrateSagaStateMachineSink<HealthSaga, EndpointTurningOff>(RemoteBus, _healthSagaRepository);
            SetupOrchestrateSagaStateMachineSink<HealthSaga, Heartbeat>(RemoteBus, _healthSagaRepository);
            SetupOrchestrateSagaStateMachineSink<HealthSaga, TimeoutExpired>(RemoteBus, _healthSagaRepository);
            SetupOrchestrateSagaStateMachineSink<HealthSaga, Pong>(RemoteBus, _healthSagaRepository);
            SetupOrchestrateSagaStateMachineSink<HealthSaga, PingTimeout>(RemoteBus, _healthSagaRepository);
            

            HealthService = new HealthService(RemoteBus, _healthSagaRepository);

            HealthService.Start();
        }

        public ISagaRepository<HealthSaga> Repository
        {
            get
            {
                return _healthSagaRepository;
            }
        }

        protected override void TeardownContext()
        {
            RemoteBus.Dispose();
            RemoteBus = null;

            LocalBus.Dispose();
            LocalBus = null;

            Thread.Sleep(500);

            HealthService.Stop();
            HealthService.Dispose();
            HealthService = null;

            base.TeardownContext();
        }
    }
}