namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using MassTransit.Testing;
    using NUnit.Framework;


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
            var secondHarness = new RabbitMqTestHarness { CleanVirtualHost = false };

            try
            {
                Assert.That(async () =>
                {
                    using var token = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                    await secondHarness.Start(token.Token).OrCanceled(TestCancellationToken);

                    await secondHarness.Stop();
                }, Throws.TypeOf<RabbitMqConnectionException>());
            }
            finally
            {
                secondHarness.Dispose();
            }
        }
    }
}
