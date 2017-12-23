namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System;
    using System.Threading.Tasks;

    using MassTransit.EntityFrameworkCoreIntegration.Saga;
    using MassTransit.Saga;
    using MassTransit.TestFramework;
    using MassTransit.Tests.Saga;
    using MassTransit.Tests.Saga.Messages;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    using NUnit.Framework;

    using Shouldly;
    using Testing;


    [TestFixture, Category("Integration")]
    public class Locating_an_existing_ef_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            Guid sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await this.InputQueueSendEndpoint.Send(message);

            Guid? foundId = await this._sagaRepository.Value.ShouldContainSaga(message.CorrelationId, this.TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await this.InputQueueSendEndpoint.Send(nextMessage);

            foundId = await this._sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, this.TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            Guid sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await this.InputQueueSendEndpoint.Send(message);

            Guid? foundId = await this._sagaRepository.Value.ShouldContainSaga(message.CorrelationId, this.TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        readonly Func<DbContext> _sagaDbContextFactory;

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public Locating_an_existing_ef_saga()
        {
            // add new migration by calling 
            // dotnet ef migrations add --context "SagaDbContext``2" Init  -v
            var contextFactory = new ContextFactory();

            using (var context = contextFactory.CreateDbContext(Array.Empty<string>()))
            {
                context.Database.Migrate();
            }

            this._sagaDbContextFactory = () => contextFactory.CreateDbContext(Array.Empty<string>());
            this._sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => 
                new EntityFrameworkSagaRepository<SimpleSaga>(this._sagaDbContextFactory));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(this._sagaRepository.Value);
        }
    }
}
