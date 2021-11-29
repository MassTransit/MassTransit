namespace MassTransit.NHibernateIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Tests.Saga;
    using MassTransit.Tests.Saga.Messages;
    using NHibernate;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("Integration")]
    public class Locating_an_existing_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public Locating_an_existing_saga()
        {
            _provider = new SQLiteSessionFactoryProvider(false, typeof(SimpleSagaMap));
            _sessionFactory = _provider.GetSessionFactory();
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => NHibernateSagaRepository<SimpleSaga>.Create(_sessionFactory));
        }

        [OneTimeSetUp]
        public void Setup()
        {
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _sessionFactory?.Dispose();
            _provider?.Dispose();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }

        readonly ISessionFactory _sessionFactory;
        readonly SQLiteSessionFactoryProvider _provider;
    }
}
