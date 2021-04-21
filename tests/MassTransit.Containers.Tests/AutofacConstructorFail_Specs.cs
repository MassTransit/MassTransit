namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Autofac_missing_dependency :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_using_the_first_consumer()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            await fault;
        }

        readonly IContainer _container;

        public Autofac_missing_dependency()
        {
            _container = new ContainerBuilder()
                .AddMassTransit(x =>
                {
                    x.AddConsumer<MyConsumer>();

                    x.AddBus(provider => BusControl);
                }).Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumers(_container.Resolve<IBusRegistrationContext>());
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
                return TaskUtil.Completed;
            }
        }
    }
}
