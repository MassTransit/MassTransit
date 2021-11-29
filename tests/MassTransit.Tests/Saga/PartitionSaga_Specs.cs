namespace MassTransit.Tests.Saga
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Partitioning_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_initiate_the_saga()
        {
            var timer = Stopwatch.StartNew();

            var ids = new Guid[Limit];
            for (var i = 0; i < Limit; i++)
            {
                var correlationId = NewId.NextGuid();
                await Bus.Publish(new CreateSaga { CorrelationId = correlationId });
                ids[i] = correlationId;
            }

            for (var i = 0; i < Limit; i++)
            {
                Guid? guid = await _repository.ShouldContainSaga(ids[i], TestTimeout);
                Assert.IsTrue(guid.HasValue);
            }

            timer.Stop();

            Console.WriteLine("Total time: {0}", timer.Elapsed);
        }

        InMemorySagaRepository<LegacySaga> _repository;

        const int Limit = 100;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _repository = new InMemorySagaRepository<LegacySaga>();

            configurator.Saga(_repository, x =>
            {
                x.Message<CreateSaga>(m => m.UsePartitioner(4, p => p.Message.CorrelationId));
            });
        }


        class LegacySaga :
            ISaga,
            InitiatedBy<CreateSaga>
        {
            public Task Consume(ConsumeContext<CreateSaga> context)
            {
                return Task.CompletedTask;
            }

            public Guid CorrelationId { get; set; }
        }


        class CreateSaga :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
