namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using MassTransit.Internals;
    using RabbitMQ.Client;
    using TestFramework.Messages;

    [TestFixture]
    public class ReconnectBug_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_refresh_credentials_and_reconnect()
        {
            var refreshed = new TaskCompletionSource<bool>();

            _refreshCallback = factory =>
            {
                factory.Password = "guest";
                refreshed.TrySetResult(true);
                return Task.CompletedTask;
            };

            await Bus.Publish(new PingMessage());

            await Task.Delay(500);

            await BusControl.StopAsync(TestCancellationToken);

            await BusControl.StartAsync(TestCancellationToken);

            Assert.That(await refreshed.Task.OrTimeout(5000), Is.True, "Refresh callback called");
        }

        Func<ConnectionFactory, Task> _refreshCallback;

        protected override void ConfigureRabbitMqHost(IRabbitMqHostConfigurator configurator)
        {
            configurator.OnRefreshConnectionFactory = factory => _refreshCallback?.Invoke(factory) ?? Task.CompletedTask;

            base.ConfigureRabbitMqHost(configurator);
        }
    }
}
