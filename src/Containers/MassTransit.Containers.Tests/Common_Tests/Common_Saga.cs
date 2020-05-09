namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using Shouldly;
    using TestFramework;
    using Testing;


    public abstract class Common_Saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_first_message()
        {
            Guid sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task Should_handle_second_message()
        {
            Guid sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var nextMessage = new SecondSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(x => x.CorrelationId == sagaId && x.Second.IsCompleted, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task Should_handle_third_message()
        {
            Guid sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var nextMessage = new ThirdSagaMessage {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(x => x.CorrelationId == sagaId && x.Third.IsCompleted, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.AddSaga<SimpleSaga>()
                .InMemoryRepository();

            configurator.AddBus(provider => BusControl);
        }

        protected abstract ISagaRepository<T> GetSagaRepository<T>()
            where T : class, ISaga;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<SimpleSaga>(Registration);
        }

        protected abstract IRegistration Registration { get; }
    }


    public abstract class Common_Saga_Endpoint :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_the_message()
        {
            Guid sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            var sendEndpoint = await Bus.GetSendEndpoint(new Uri("loopback://localhost/custom-endpoint-name"));

            await sendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task Should_use_custom_endpoint_and_definition_together()
        {
            Guid sagaId = NewId.NextGuid();

            var message = new FirstSagaMessage {CorrelationId = sagaId};

            var sendEndpoint = await Bus.GetSendEndpoint(new Uri("loopback://localhost/custom-second-saga"));

            await sendEndpoint.Send(message);

            Guid? foundId = await GetSagaRepository<SecondSimpleSaga>().ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.AddSaga<SimpleSaga>()
                .Endpoint(e => e.Name = "custom-endpoint-name")
                .InMemoryRepository();

            configurator.AddSaga<SecondSimpleSaga>(typeof(SecondSimpleSagaDefinition))
                .Endpoint(e => e.Temporary = true)
                .InMemoryRepository();

            configurator.AddBus(provider => BusControl);
        }

        protected abstract ISagaRepository<T> GetSagaRepository<T>()
            where T : class, ISaga;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }

        protected abstract IRegistration Registration { get; }
    }
}
