namespace MassTransit.Tests.Testing
{
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class BusPublish_Specs
    {
        [Test]
        public async Task Should_observe_published_event()
        {
            //Arrange
            var provider = new ServiceCollection()
                .AddScoped<IMyApplicationService, MyApplicationService>()
                .AddMassTransitTestHarness()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var service = harness.Scope.ServiceProvider.GetRequiredService<IMyApplicationService>();

            await service.Register();

            Assert.That(await harness.Published.Any<MyMessage>());
        }


        public class MyMessage
        {
        }


        public interface IMyApplicationService
        {
            Task Register();
        }


        class MyApplicationService :
            IMyApplicationService
        {
            readonly IBus _bus;

            public MyApplicationService(IBus bus)
            {
                _bus = bus;
            }

            public async Task Register()
            {
                await _bus.Publish(new MyMessage());
            }
        }
    }
}
