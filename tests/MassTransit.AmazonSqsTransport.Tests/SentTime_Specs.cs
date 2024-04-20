namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Serialization;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class SentTime_Specs
    {
        [Test]
        public async Task Should_have_sent_time_header_in_utc()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (PingMessage _) =>
                    {
                    });

                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.LocalstackHost();

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish(new PingMessage());

            IReceivedMessage<PingMessage> consumed = await harness.Consumed.SelectAsync<PingMessage>().FirstOrDefault();
            Assert.That(consumed, Is.Not.Null);

            Assert.That(consumed.Context.SentTime.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test]
        public async Task Should_have_sent_time_header_in_utc_using_raw_json()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (PongMessage _) =>
                    {
                    });

                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.LocalstackHost();

                        cfg.UseRawJsonSerializer();

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish(new PongMessage());

            IReceivedMessage<PongMessage> consumed = await harness.Consumed.SelectAsync<PongMessage>().FirstOrDefault();
            Assert.That(consumed, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(consumed.Context.SentTime.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(consumed.Context.ReceiveContext.ContentType, Is.EqualTo(SystemTextJsonRawMessageSerializer.JsonContentType));

                Assert.That(consumed.Context.ReceiveContext.GetSentTime(), Is.GreaterThanOrEqualTo(consumed.Context.SentTime - TimeSpan.FromSeconds(30)));
            });
            Assert.That(consumed.Context.ReceiveContext.GetSentTime(), Is.LessThanOrEqualTo(consumed.Context.SentTime + TimeSpan.FromSeconds(30)));
        }
    }
}
