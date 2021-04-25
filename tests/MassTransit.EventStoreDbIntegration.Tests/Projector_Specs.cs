namespace MassTransit.EventStoreDbIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EventStore.Client;
    using GreenPipes;
    using MassTransit.Context;
    using MassTransit.EventStoreDbIntegration.Contexts;
    using MassTransit.EventStoreDbIntegration.Serializers;
    using MassTransit.Serialization;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;


    public class Projector_Specs :
        InMemoryTestFixture
    {
        const string SubscriptionName = "mt_projector_test";

        [Test]
        public async Task Should_project()
        {
            TaskCompletionSource<ConsumeContext<ProjectorTaskCompleted>> taskCompletionSource = GetTask<ConsumeContext<ProjectorTaskCompleted>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            _ = services.AddSingleton<EventStoreClient>((provider) =>
            {
                var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
                settings.ConnectionName = "MassTransit Test Connection";

                return new EventStoreClient(settings);
            });

            var orderItemsRepository = new Dictionary<Guid, List<OrderItem>>();
            var placedOrdersRepository = new Dictionary<Guid, PlacedOrder>();

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));

                x.AddRider(rider =>
                {
                    rider.AddConsumer<OrderItemsProjector>();
                    rider.AddConsumer<PlacedOrdersProjector>();
                    rider.AddConsumer<ProjectorTaskCompletedConsumer>();

                    rider.UsingEventStoreDB((context, esdb) =>
                    {
                        esdb.CatchupSubscription(StreamName.AllStream, SubscriptionName, c =>
                        {
                            c.UseRawJsonSerializer(RawJsonSerializerOptions.CopyHeaders);

                            c.CheckpointMessageCount = 5;
                            c.UseEventStoreDBCheckpointStore(StreamName.ForCheckpoint(SubscriptionName));

                            c.AllStreamEventFilter = EventTypeFilter.RegularExpression("^Order");

                            c.Instance(new OrderItemsProjector(orderItemsRepository));
                            c.Instance(new PlacedOrdersProjector(placedOrdersRepository));
                        });

                        esdb.CatchupSubscription(StreamName.AllStream, $"{SubscriptionName}_tcs", c =>
                        {
                            c.UseJsonSerializer();

                            c.CheckpointMessageCount = 1;
                            c.UseEventStoreDBCheckpointStore(StreamName.ForCheckpoint($"{SubscriptionName}_tcs"));

                            c.AllStreamEventFilter = EventTypeFilter.RegularExpression("^Projector");

                            c.ConfigureConsumer<ProjectorTaskCompletedConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                using var producer = provider.GetRequiredService<EventStoreClient>();

                var serializer = new RawJsonMessageSerializer();

                List<EventData> preparedMessages = new List<EventData>();

                var orderId = Guid.NewGuid();
                var buyerId = Guid.NewGuid();
                var orderStreamName = StreamName.Custom($"order-{orderId}");

                var itemId_1 = Guid.NewGuid();
                var itemId_2 = Guid.NewGuid();
                var itemId_3 = Guid.NewGuid();
                var itemId_4 = Guid.NewGuid();
                var itemId_5 = Guid.NewGuid();

                var conversationId = Guid.NewGuid(); //Equivalent to EventStoreDB $correlationId
                //var initiatorId = conversationId;  //Equivalent to EventStoreDB $causationId
                var correlationId = Guid.NewGuid();  //The instance id used for correlation in sagas

                preparedMessages.Add(await EventUtils.PrepareMessage(new OrderCreated(orderId, buyerId, DateTime.UtcNow), serializer, conversationId, conversationId, correlationId));
                preparedMessages.Add(await EventUtils.PrepareMessage(new OrderItemAdded(orderId, itemId_1), serializer, conversationId, conversationId, correlationId));
                preparedMessages.Add(await EventUtils.PrepareMessage(new OrderItemAdded(orderId, itemId_2), serializer, conversationId, conversationId, correlationId));
                preparedMessages.Add(await EventUtils.PrepareMessage(new OrderItemAdded(orderId, itemId_3), serializer, conversationId, conversationId, correlationId));
                preparedMessages.Add(await EventUtils.PrepareMessage(new OrderItemAdded(orderId, itemId_4), serializer, conversationId, conversationId, correlationId));
                preparedMessages.Add(await EventUtils.PrepareMessage(new OrderItemAdded(orderId, itemId_5), serializer, conversationId, conversationId, correlationId));
                await producer.AppendToStreamAsync(orderStreamName, StreamState.Any, preparedMessages);

                conversationId = Guid.NewGuid();
                preparedMessages.Clear();
                preparedMessages.Add(await EventUtils.PrepareMessage(new OrderItemRemoved(orderId, itemId_2), serializer, conversationId, conversationId, correlationId));
                await producer.AppendToStreamAsync(orderStreamName, StreamState.Any, preparedMessages);

                conversationId = Guid.NewGuid();
                preparedMessages.Clear();
                preparedMessages.Add(await EventUtils.PrepareMessage(new OrderItemRemoved(orderId, itemId_3), serializer, conversationId, conversationId, correlationId));
                await producer.AppendToStreamAsync(orderStreamName, StreamState.Any, preparedMessages);

                conversationId = Guid.NewGuid();
                preparedMessages.Clear();
                preparedMessages.Add(await EventUtils.PrepareMessage(new OrderItemRemoved(orderId, itemId_5), serializer, conversationId, conversationId, correlationId));
                await producer.AppendToStreamAsync(orderStreamName, StreamState.Any, preparedMessages);

                conversationId = Guid.NewGuid();
                preparedMessages.Clear();
                preparedMessages.Add(await EventUtils.PrepareMessage(new OrderPlaced(orderId), serializer, conversationId, conversationId, correlationId));
                await producer.AppendToStreamAsync(orderStreamName, StreamState.Any, preparedMessages);

                var messageId = Guid.NewGuid();
                conversationId = Guid.NewGuid();
                preparedMessages.Clear();
                preparedMessages.Add(await EventUtils.PrepareMessage(new ProjectorTaskCompleted("text"), new JsonMessageSerializer(), conversationId, conversationId, correlationId, messageId));

                var tcsStreamName = StreamName.Custom($"order_tcs-{orderId}");
                await producer.AppendToStreamAsync(tcsStreamName, StreamState.Any, preparedMessages);

                ConsumeContext<ProjectorTaskCompleted> result = await taskCompletionSource.Task;

                Assert.AreEqual("text", result.Message.Text);
                Assert.That(result.MessageId, Is.EqualTo(messageId));
                Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
                Assert.That(result.InitiatorId, Is.EqualTo(conversationId));
                Assert.That(result.ConversationId, Is.EqualTo(conversationId));

                Assert.That(orderItemsRepository[orderId].Count(), Is.EqualTo(2));
                Assert.That(placedOrdersRepository.Count(), Is.EqualTo(1));
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }


        public class OrderItemsProjector :
            IConsumer<OrderCreated>,
            IConsumer<OrderItemAdded>,
            IConsumer<OrderItemRemoved>,
            IConsumer<OrderCancelled>
        {
            readonly Dictionary<Guid, List<OrderItem>> _repository;

            public OrderItemsProjector(Dictionary<Guid, List<OrderItem>> repository)
            {
                _repository = repository;
            }

            public Task Consume(ConsumeContext<OrderCreated> context)
            {
                var e = context.Message;

                _repository.Add(e.OrderId, new List<OrderItem>());

                return Task.CompletedTask;
            }

            public Task Consume(ConsumeContext<OrderItemAdded> context)
            {
                var e = context.Message;

                var items = _repository[e.OrderId];

                items.Add(new OrderItem
                {
                    OrderId = e.OrderId,
                    ItemId = e.ItemId
                });

                return Task.CompletedTask;
            }

            public Task Consume(ConsumeContext<OrderItemRemoved> context)
            {
                var e = context.Message;

                var items = _repository[e.OrderId];
                var item = items.Where(i => i.ItemId == e.ItemId).First();
                items.Remove(item);

                return Task.CompletedTask;
            }

            public Task Consume(ConsumeContext<OrderCancelled> context)
            {
                var e = context.Message;

                _repository.Remove(e.OrderId);

                return Task.CompletedTask;
            }
        }

        public class OrderItem
        {
            public Guid OrderId { get; set; }
            public Guid ItemId { get; set; }
        }


        public class PlacedOrdersProjector :
            IConsumer<OrderCreated>,
            IConsumer<OrderPlaced>,
            IConsumer<OrderCancelled>
        {
            readonly Dictionary<Guid, PlacedOrder> _repository;

            public PlacedOrdersProjector(Dictionary<Guid, PlacedOrder> repository)
            {
                _repository = repository;
            }

            public Task Consume(ConsumeContext<OrderCreated> context)
            {
                var e = context.Message;

                _repository.Add(e.OrderId, new PlacedOrder(e.OrderId, e.BuyerId));

                return Task.CompletedTask;
            }

            public Task Consume(ConsumeContext<OrderPlaced> context)
            {
                var e = context.Message;

                _repository[e.OrderId].IsPlaced = true;

                return Task.CompletedTask;
            }

            public Task Consume(ConsumeContext<OrderCancelled> context)
            {
                var e = context.Message;

                _repository.Remove(e.OrderId);

                return Task.CompletedTask;
            }
        }

        public class PlacedOrder
        {
            public PlacedOrder(Guid orderId, Guid buyerId)
            {
                OrderId = orderId;
                BuyerId = buyerId;
                IsPlaced = false;
            }

            public Guid OrderId { get; set; }
            public Guid BuyerId { get; set; }
            public bool IsPlaced { get; set; }
        }


        public class ProjectorTaskCompletedConsumer :
            IConsumer<ProjectorTaskCompleted>
        {
            readonly TaskCompletionSource<ConsumeContext<ProjectorTaskCompleted>> _taskCompletionSource;

            public ProjectorTaskCompletedConsumer(TaskCompletionSource<ConsumeContext<ProjectorTaskCompleted>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<ProjectorTaskCompleted> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }

        public class ProjectorTaskCompleted
        {
            public ProjectorTaskCompleted(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }
    }
}
