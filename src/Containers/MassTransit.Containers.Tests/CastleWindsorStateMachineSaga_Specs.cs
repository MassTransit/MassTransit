using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Containers.Tests
{
    using Automatonymous;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using NUnit.Framework;
    using Saga;
    using Scenarios.StateMachines;
    using TestFramework;
    using AutomatonymousWindsorIntegration;

    public class CastleWindsorStateMachineSaga_Specs : InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_first_event_successfully()
        {
            Task<ConsumeContext<TestStarted>> started = ConnectPublishHandler<TestStarted>();
            Task<ConsumeContext<TestUpdated>> updated = ConnectPublishHandler<TestUpdated>();

            await InputQueueSendEndpoint.Send(new StartTest { CorrelationId = NewId.NextGuid(), TestKey = "Unique" });

            await started;

            await InputQueueSendEndpoint.Send(new UpdateTest { TestKey = "Unique" });

            await updated;
        }

        IWindsorContainer _container;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _container = new WindsorContainer();

            _container.Register(Component.For(typeof(ISagaRepository<>))
                .ImplementedBy(typeof(InMemorySagaRepository<>)));

            _container.Register(Component.For<PublishTestStartedActivity>()
                .LifestyleTransient());

            _container.RegisterStateMachineSagas(GetType().Assembly);

            configurator.LoadStateMachineSagas(_container);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }
    }
}
