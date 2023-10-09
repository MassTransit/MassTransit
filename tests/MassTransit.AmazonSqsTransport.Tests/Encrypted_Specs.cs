namespace MassTransit.AmazonSqsTransport.Tests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class Using_an_encrypted_binary_serializer
    {
        [Test]
        public async Task Should_properly_expand_the_base64_string()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<EncryptedConsumer>();

                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.LocalstackHost();

                        cfg.UseEncryption(_key);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish(new EncryptedString { Text = "Howdy!" });

            Assert.That(await harness.Consumed.Any<EncryptedString>(x => x.Exception == null), Is.True);

            ConsumeContext<EncryptedString> context = (await harness.Consumed.SelectAsync<EncryptedString>(x => x.Exception == null).FirstOrDefault())?.Context;

            Assert.That(context, Is.Not.Null);

            Assert.That(context.Message.Text, Is.EqualTo("Howdy!"));
        }

        static readonly byte[] _key =
        {
            31,
            182,
            254,
            29,
            98,
            114,
            85,
            168,
            176,
            48,
            113,
            206,
            198,
            176,
            181,
            125,
            106,
            134,
            98,
            217,
            113,
            158,
            88,
            75,
            118,
            223,
            117,
            160,
            224,
            1,
            47,
            162
        };


        public class EncryptedString
        {
            public string Text { get; set; }
        }


        class EncryptedConsumer :
            IConsumer<EncryptedString>
        {
            public Task Consume(ConsumeContext<EncryptedString> context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
