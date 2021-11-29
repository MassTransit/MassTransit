namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    public class When_a_dependency_is_missing<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_using_the_first_consumer()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            await fault;
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<MyConsumer>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumers(BusRegistrationContext);
        }


        public interface IMyDependency
        {
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            public MyConsumer(IMyDependency dependency)
            {
            }

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
