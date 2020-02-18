namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework.Messages;


    [TestFixture]
    public class AutofacContainer_RegistrationExtension
    {
        [Test]
        public void Registration_extension_method_for_consumers()
        {
            var container = new ContainerBuilder()
                .AddMassTransit(cfg => cfg.AddConsumersFromNamespaceContaining<AutofacContainer_RegistrationExtension>())
                .Build();

            Assert.That(container.IsRegistered<TestConsumer>(), Is.True);
        }

        [Test]
        public async Task Throw_them_under_the_bus()
        {
            var container = new ContainerBuilder().AddMassTransit(cfg =>
                {
                    cfg.AddConsumersFromNamespaceContaining<AutofacContainer_RegistrationExtension>();
                    cfg.AddSaga<SimpleSaga>()
                        .InMemoryRepository();
                }).Build();

            var busControl = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.ConfigureConsumers(container);
                    e.ConfigureSagas(container);
                });
            });

            var busHandle = await busControl.StartAsync();

            await busHandle.Ready;

            await busHandle.StopAsync();
        }
    }


    public class TestConsumer :
        IConsumer<PingMessage>
    {
        public async Task Consume(ConsumeContext<PingMessage> context)
        {
        }
    }
}
