namespace MassTransit.RedisIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Saga;
    using NUnit.Framework;
    using Shouldly;
    using StackExchange.Redis;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("Integration")]
    public class LocatingAnExistingSaga :
        InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            var found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            found = await _sagaRepository.Value.ShouldContainSaga(sagaId, x => x != null && x.Moved, TestTimeout);
            found.ShouldNotBeNull();

            var retrieveRepository = _sagaRepository.Value as ILoadSagaRepository<SimpleSaga>;
            var retrieved = await retrieveRepository.Load(sagaId);
            retrieved.ShouldNotBeNull();
            retrieved.Moved.ShouldBeTrue();
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            var found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public LocatingAnExistingSaga()
        {
            var redis = ConnectionMultiplexer.Connect("127.0.0.1");
            redis.PreserveAsyncOrder = false;

            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => new RedisSagaRepository<SimpleSaga>(() => redis.GetDatabase()));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }

    [TestFixture]
    [Category("Integration")]
    public class LocatingAnExistingSagaWithoutOptimism :
        InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            var found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            found = await _sagaRepository.Value.ShouldContainSaga(sagaId, x => x != null && x.Moved, TestTimeout);
            found.ShouldNotBeNull();

            var retrieveRepository = _sagaRepository.Value as ILoadSagaRepository<SimpleSaga>;
            var retrieved = await retrieveRepository.Load(sagaId);
            retrieved.ShouldNotBeNull();
            retrieved.Moved.ShouldBeTrue();
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            var found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public LocatingAnExistingSagaWithoutOptimism()
        {
            var redis = ConnectionMultiplexer.Connect("127.0.0.1");
            redis.PreserveAsyncOrder = false;

            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => new RedisSagaRepository<SimpleSaga>(() => redis.GetDatabase(), optimistic: false));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }

    [TestFixture]
    [Category("Integration")]
    public class LocatingAnExistingSagaWithKeyPrefix :
       InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            var found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            found = await _sagaRepository.Value.ShouldContainSaga(sagaId, x => x != null && x.Moved, TestTimeout);
            found.ShouldNotBeNull();

            var retrieveRepository = _sagaRepository.Value as ILoadSagaRepository<SimpleSaga>;
            var retrieved = await retrieveRepository.Load(sagaId);
            retrieved.ShouldNotBeNull();
            retrieved.Moved.ShouldBeTrue();
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            var found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public LocatingAnExistingSagaWithKeyPrefix()
        {
            var redis = ConnectionMultiplexer.Connect("127.0.0.1");
            redis.PreserveAsyncOrder = false;

            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => new RedisSagaRepository<SimpleSaga>(() => redis.GetDatabase(), keyPrefix: "test"));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}
