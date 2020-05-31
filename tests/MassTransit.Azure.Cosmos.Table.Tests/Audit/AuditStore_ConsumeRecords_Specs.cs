namespace MassTransit.Azure.Cosmos.Table.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes.Util;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Saving_consume_audit_records_to_the_audit_store :
        AzureCosmosTableInMemoryTestFixture
    {
        [Test]
        public async Task Should_have_consume_audit_records()
        {
            IEnumerable<AuditRecord> consumeRecords = AzureTableHelpers.GetAuditRecords().Where(x => x.ContextType == "Consume");
            consumeRecords.Count().ShouldBe(2);
        }

        Task<ConsumeContext<A>> _handledA;
        Task<ConsumeContext<B>> _handledB;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await InputQueueSendEndpoint.Send(new A());
            await _handledA;
            await InputQueueSendEndpoint.Send(new B());
            await _handledB;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseAzureCosmosTableAuditStore(configure => configure.WithConnectionString(ConnectionString)
                                                                             .WithTableName(AuditTableName)
                                                                             .WithContextTypePartitionKeyStrategy()
                                                                             .WithNoMessageFilter()
                                                                             .Build());
            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<TestConsumer>();
            _handledA = Handled<A>(configurator);
            _handledB = Handled<B>(configurator);
        }


        class TestConsumer : IConsumer<A>,
                             IConsumer<B>
        {
            public Task Consume(ConsumeContext<A> context)
            {
                return TaskUtil.Completed;
            }

            public Task Consume(ConsumeContext<B> context)
            {
                return TaskUtil.Completed;
            }
        }


        class A
        {
            public string Data { get; set; }
        }


        class B
        {
            public string Data { get; set; }
        }
    }
}
