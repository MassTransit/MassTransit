namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    [Explicit]
    public class PublishTimeout_Specs :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_fault_with_operation_cancelled_on_publish()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);

                x.Host("unknown_host");

                x.AutoStart = true;
            });

            using var startTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));

            Task<BusHandle> startTask = busControl.StartAsync(startTimeout.Token).OrCanceled(TestCancellationToken);

            var publishTimer = Stopwatch.StartNew();
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                using var publishTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await busControl.Publish(new PingMessage(), publishTimeout.Token);
            });

            publishTimer.Stop();
            var publishElapsed = publishTimer.Elapsed;

            Assert.That(publishElapsed, Is.LessThan(TimeSpan.FromSeconds(19)));

            Assert.ThrowsAsync<RabbitMqConnectionException>(async () =>
            {
                await startTask;
            });
        }

        public PublishTimeout_Specs()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
