namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Tests.Saga;
    using MassTransit.Tests.Saga.Messages;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using Saga;
    using Shouldly;
    using TestFramework;
    using Testing;


    [TestFixture, Category("Integration")]
    public class Using_custom_include_in_repository :
        InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_update_inner_saga_dependency()
        {
            Guid sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var propertyValue = "expected saga property value";
            var updateInnerProperty = new UpdateSagaDependency(sagaId, propertyValue);

            await InputQueueSendEndpoint.Send(updateInnerProperty);

            foundId = await _sagaRepository.Value.ShouldContainSaga(
                x => x.CorrelationId == sagaId && x.Completed && x.Dependency.SagaInnerDependency.Name == propertyValue, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            Guid sagaId = NewId.NextGuid();
            Console.WriteLine(sagaId);
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message).ConfigureAwait(false);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout).ConfigureAwait(false);

            foundId.HasValue.ShouldBe(true);
        }

        readonly Func<DbContext> _sagaDbContextFactory;

        readonly Lazy<ISagaRepository<SagaWithDependency>> _sagaRepository;

        public Using_custom_include_in_repository()
        {
            // add new migration by calling 
            // dotnet ef migrations add --context "SagaWithDependencyContext" Init  -v
            var contextFactory = new SagaWithDependencyContextFactory();

            using (var context = contextFactory.CreateDbContext(Array.Empty<string>()))
            {
                context.Database.Migrate();
            }

            _sagaDbContextFactory = () => contextFactory.CreateDbContext(Array.Empty<string>());
            _sagaRepository = new Lazy<ISagaRepository<SagaWithDependency>>(() =>
                EntityFrameworkSagaRepository<SagaWithDependency>.CreatePessimistic(_sagaDbContextFactory,
                    queryCustomization: queryable =>
                        queryable.Include(it => it.Dependency).ThenInclude(dependency => dependency.SagaInnerDependency)
                    ));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}
