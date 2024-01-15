#nullable enable
namespace MassTransit.AmazonSqsTransport.Tests.Persistence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class Storage_Specs
    {
        [Test]
        public async Task S3MessageDataTestLoadFetchAsync()
        {
            Environment.SetEnvironmentVariable("AWS_REGION", "us-east-1");
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "admin");
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "admin");

            MessageDataDefaults.TimeToLive = TimeSpan.FromDays(2);
            MessageDataDefaults.ExtraTimeToLive = TimeSpan.FromMinutes(5);

            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetKebabCaseEndpointNameFormatter();
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseMessageData(d => d.AmazonS3("test", new AmazonS3Config
                        {
                            ForcePathStyle = true,
                            ServiceURL = "http://localhost:4566",
                        }));
                        cfg.ConfigureEndpoints(context);
                    });
                    x.AddConsumer<EmptyConsumer>();
                }).BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();

            var client = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var randomText = GenerateRandomAlphanumericString(7000);
            await client.Publish<SimpleMessage>(new
            {
                BigData = randomText,
            });

            Assert.That(await harness.Consumed.Any<SimpleMessage>(), Is.True, "Did not receive message");
            IReceivedMessage<SimpleMessage>? receivedMessage = await harness.Consumed.SelectAsync<SimpleMessage>().First();
            var message = receivedMessage.Context.Message;
            Assert.That(message.BigData, Is.Not.Null);
            var messageValue = await message.BigData!.Value;
            Assert.That(messageValue, Is.EqualTo(randomText), "Message not retrieved");

            await harness.Stop();
        }

        static string GenerateRandomAlphanumericString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
    }
}
