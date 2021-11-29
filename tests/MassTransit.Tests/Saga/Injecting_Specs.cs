namespace MassTransit.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Messages;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Injecting_properties_into_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task The_saga_should_be_created_when_an_initiating_message_is_received()
        {
            var message = new InitiateSimpleSaga(_sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? sagaId = await _repository.ShouldContainSaga(_sagaId, TestTimeout);

            sagaId.HasValue.ShouldBe(true);
        }

        ISagaRepository<InjectingSampleSaga> _repository;
        Dependency _dependency;
        Guid _sagaId;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _sagaId = NewId.NextGuid();

            // this is our dependency, but could be dynamically resolved from a container in method
            // below is so desired.
            _dependency = new Dependency();

            // create the actual saga repository
            _repository = SetupSagaRepository<InjectingSampleSaga>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_repository, x => x.UseExecute(context => context.Saga.Dependency = _dependency));
        }

        static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>()
            where TSaga : class, ISaga
        {
            return new InMemorySagaRepository<TSaga>();
        }


        class Dependency :
            IDependency
        {
        }
    }
}
