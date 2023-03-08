namespace MassTransit.AmazonS3.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Configuration;
    using Testing;

    [TestFixture]
    public class Storage_Specs
    {
        IServiceProvider _provider;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("AWS_REGION", "us-east-1");
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "admin");
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "admin");
            MessageDataDefaults.TimeToLive = TimeSpan.FromDays(2);
            MessageDataDefaults.ExtraTimeToLive = TimeSpan.FromMinutes(5);
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetKebabCaseEndpointNameFormatter();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseMessageData(d => d.AmazonS3Storage("test", new AmazonS3Config()
                        {
                            ForcePathStyle = true,
                            ServiceURL = "http://localhost:4566",
                        }));
                        cfg.ConfigureEndpoints(context);
                    });
                    x.AddConsumer<EmptyConsumer>();
                }).BuildServiceProvider();
        }

        [Test]
        public async Task S3MessageDataTestLoadFetchAsync()
        {
            var harness = _provider.GetTestHarness();
            await harness.Start();
            var client = _provider.CreateScope().ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var randomText = GenerateRandomAlphanumericString(7000);
            await client.Publish<SimpleMessage>(new
            {
                BigData = randomText,
            });
            Assert.That(await harness.Consumed.Any<SimpleMessage>(), Is.True, "Did not receive message");
            var message = await harness.Consumed.SelectAsync<SimpleMessage>().First();
            var messageValue = await ((SimpleMessage)message.MessageObject).BigData.Value;
            Assert.That(messageValue, Is.EqualTo(randomText), "Message not retrieved");

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
