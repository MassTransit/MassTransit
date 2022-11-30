namespace MassTransit.Tests.MessageData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.MessageData;
    using MassTransit.Testing;
    using MessageDataComponents;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using NUnit.Framework;


    [TestFixture]
    public class Using_message_data_with_json_serializers
    {
        [Test]
        public async Task Should_work_with_System_Text_Json()
        {
            await using var provider = new ServiceCollection()
                .AddSingleton<IMessageDataRepository, InMemoryMessageDataRepository>()
                .AddMassTransitTestHarness(x =>
                {
                    MessageDataDefaults.AlwaysWriteToRepository = false;
                    MessageDataDefaults.TimeToLive = TimeSpan.FromDays(7);
                    MessageDataDefaults.Threshold = 1;

                    x.AddConsumer<SampleItemsCreatedConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseMessageData(context.GetRequiredService<IMessageDataRepository>());

                        cfg.UseNewtonsoftJsonSerializer();
                        cfg.ConfigureNewtonsoftJsonSerializer(settings =>
                        {
                            settings.DefaultValueHandling = DefaultValueHandling.Populate;
                            settings.Converters.Add(new StringEnumConverter());

                            return settings;
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider();

            var harness = provider.GetTestHarness();
            await harness.Start();

            IEnumerable<SampleItem> CreateSampleItems(int quantity)
            {
                for (var i = 0; i < quantity; i++)
                    yield return new SampleItem(Guid.NewGuid());
            }

            var items = new[] { 1_000, 5_000, 10_00, 25_000, 50_000, 100_000, 200_000, 500_000, 1_000_000 };
            foreach (var item in items)
            {
                SampleItem[] sampleItems = CreateSampleItems(item).ToArray();

                var contract = new SampleItemsCreatedContract(Guid.NewGuid(), sampleItems, true);

                var @event = new
                {
                    Id = Guid.NewGuid(),
                    Body = contract
                };

                await harness.Bus.Publish<SampleItemsMessageDataEvent>(@event);
            }

            Assert.That(await harness.Consumed.Any<SampleItemsMessageDataEvent>(), Is.True);

            var context = await harness.Consumed.SelectAsync<SampleItemsMessageDataEvent>().First();

            Assert.That(context.Exception, Is.Null);
        }
    }


    namespace MessageDataComponents
    {
        using System;
        using System.Collections.Generic;
        using System.Diagnostics;
        using System.Linq;
        using System.Threading.Tasks;
        using MassTransit;
        using Microsoft.Extensions.Logging;


        public class SampleItemsCreatedConsumer :
            IConsumer<SampleItemsMessageDataEvent>
        {
            readonly ILogger<SampleItemsCreatedConsumer> _logger;

            public SampleItemsCreatedConsumer(ILogger<SampleItemsCreatedConsumer> logger)
            {
                _logger = logger;
            }

            public async Task Consume(ConsumeContext<SampleItemsMessageDataEvent> context)
            {
                var st = Stopwatch.StartNew();
                var message = await context.Message.Body.Value;
                st.Stop();

                _logger.LogInformation("Message #{MessageId}={Id} consumed, with items totalMessages {Count} - {Duration}ms", context.MessageId,
                    context.Message.Id, message.Messages?.Count(), st.ElapsedMilliseconds);
            }
        }


        public class SampleItem
        {
            public SampleItem(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; set; }
        }


        public class SampleItemsCreatedContract
        {
            public SampleItemsCreatedContract()
            {
            }

            [JsonConstructor]
            public SampleItemsCreatedContract(Guid messageId, IEnumerable<SampleItem> messages, bool boolExample)
            {
                MessageId = messageId;
                BatchId = BatchId;
                Messages = messages;
                BoolExample = boolExample;
            }

            public SampleItemsCreatedContract(IEnumerable<SampleItem> messages, bool boolExample)
            {
                BatchId = BatchId;
                Messages = messages;
                BoolExample = boolExample;
            }

            public Guid MessageId { get; set; }
            public Guid BatchId { get; set; }
            public IEnumerable<SampleItem> Messages { get; set; }
            public bool BoolExample { get; set; }
        }


        public class SampleItemsMessageDataEvent
        {
            public Guid Id { get; set; }
            public MessageData<SampleItemsCreatedContract> Body { get; set; }
        }
    }
}
