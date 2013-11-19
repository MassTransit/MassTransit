// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Tests.Services.HealthMonitoring
{
    using System;
    using System.Linq;
    using System.Threading;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Pipeline.Inspectors;
    using MassTransit.Saga;
    using MassTransit.Services.HealthMonitoring;
    using MassTransit.Services.HealthMonitoring.Messages;
    using MassTransit.Services.HealthMonitoring.Server;
    using MassTransit.Services.Subscriptions.Server;
    using MassTransit.Services.Timeout;
    using MassTransit.Services.Timeout.Server;
    using MassTransit.Transports.Loopback;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Fixtures;

    [TestFixture]
    public class When_using_health_monitoring
        : EndpointTestFixture<LoopbackTransportFactory>
    {
        IServiceBus _serviceBus;
        IServiceBus _subscriptionBus;
        IServiceBus _healthBus;
        IServiceBus _timeoutBus;

        TimeoutService _timeoutService;
        HealthService _healthService;
        SubscriptionService _subscriptionService;
        InMemorySagaRepository<HealthSaga> _healthSagas;

        [SetUp]
        public void Configure()
        {
            var subscriptionServiceUri = new Uri("loopback://localhost/mt_subscriptions");
            var healthUri = new Uri("loopback://localhost/mt_health");
            var timeoutUri = new Uri("loopback://localhost/mt_timeout");
            var localUri = new Uri("loopback://localhost/local");

            _subscriptionBus = SetupServiceBus(subscriptionServiceUri,
                configurator => configurator.SetConcurrentConsumerLimit(1));

            var subscriptionSagas = new InMemorySagaRepository<SubscriptionSaga>();
            var subscriptionClientSagas = new InMemorySagaRepository<SubscriptionClientSaga>();
            _subscriptionService = new SubscriptionService(_subscriptionBus,subscriptionSagas,subscriptionClientSagas);

            _subscriptionService.Start();

            _timeoutBus = SetupServiceBus(timeoutUri,
                configurator =>
                {
                    configurator.UseControlBus();
                    configurator.UseSubscriptionService(subscriptionServiceUri);
                });

            _timeoutService = new TimeoutService(_timeoutBus, new InMemorySagaRepository<TimeoutSaga>());
            _timeoutService.Start();

            Thread.Sleep(500);

            _healthBus = SetupServiceBus(healthUri,
                configurator =>
                    {
                        configurator.UseControlBus();
                        configurator.UseSubscriptionService(subscriptionServiceUri);
                    });

            _healthSagas = new InMemorySagaRepository<HealthSaga>();
            _healthService = new HealthService(_healthBus, _healthSagas);
            _healthService.Start();

            Thread.Sleep(500);

            _serviceBus = SetupServiceBus(localUri, configurator =>
                {
                    configurator.UseControlBus();
                    configurator.UseHealthMonitoring(1);
                    configurator.UseSubscriptionService(subscriptionServiceUri);
                });

            Thread.Sleep(500);
        }

        [TearDown]
        public void Cleanup()
        {
            _healthService.Stop();
            _timeoutService.Stop();
            _subscriptionService.Stop();

            _serviceBus.Dispose();
            _healthBus.Dispose();
            _timeoutBus.Dispose();
            _subscriptionBus.Dispose();
        }

        [Test]
        public void Heartbeat_subscriptions_should_be_kept_on_the_control_bus()
        {
            Console.WriteLine("data bus");
            PipelineViewer.Trace(_serviceBus.OutboundPipeline);

            Console.WriteLine("control bus");
            PipelineViewer.Trace(_serviceBus.ControlBus.OutboundPipeline);

            _serviceBus.ControlBus.ShouldHaveRemoteSubscriptionFor<Heartbeat>();
        }
    }
}