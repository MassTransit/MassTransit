namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes.Internals.Extensions;
    using NUnit.Framework;
    using RabbitMqTransport.Testing;
    using TestFramework.Logging;


    public class ExclusiveConsumer_Specs :
        RabbitMqTestFixture
    {
        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ExclusiveConsumer = true;
        }

        [Test]
        public async Task Should_not_be_allowed_twice()
        {
            var loggerFactory = new TestOutputLoggerFactory(true);

            LogContext.ConfigureCurrentLogContext(loggerFactory);

            DiagnosticListener.AllListeners.Subscribe(new DiagnosticListenerObserver());

            var secondHarness = new RabbitMqTestHarness();

            try
            {
                Assert.That(async () =>
                {
                    using (var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                    {
                        await secondHarness.Start(token.Token).OrCanceled(TestCancellationToken);

                        await secondHarness.Stop();
                    }
                }, Throws.TypeOf<RabbitMqConnectionException>());
            }
            finally
            {
                secondHarness.Dispose();
            }
        }
    }
}
