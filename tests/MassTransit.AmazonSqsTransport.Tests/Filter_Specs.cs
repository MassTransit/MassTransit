namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Filtering;
    using NUnit.Framework;
    using Util;


    namespace Filtering
    {
        public class Message
        {
            public Message(string key, string value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; set; }
            public string Value { get; set; }
        }
    }


    [TestFixture]
    public class Using_a_filter_on_an_endpoint :
        AmazonSqsTestFixture
    {
        [Test]
        public async Task Should_support_a_queue_with_a_filter()
        {
            var fooHandle = await Subscribe("foo");
            var barHandle = await Subscribe("bar");

            await Bus.Publish(new Message("foo", "Hello"), x => x.Headers.Set("RoutingKey", "foo"));
            await Bus.Publish(new Message("bar", "World"), x => x.Headers.Set("RoutingKey", "bar"));

            await _foo.Task;
            await _bar.Task;
        }

        readonly TaskCompletionSource<Message> _foo;
        readonly TaskCompletionSource<Message> _bar;

        public Using_a_filter_on_an_endpoint()
        {
            _foo = GetTask<Message>();
            _bar = GetTask<Message>();
        }

        async Task<HostReceiveEndpointHandle> Subscribe(string key)
        {
            var queueName = $"TestCase-R-{key}";
            var handle = Bus.ConnectReceiveEndpoint(queueName, x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Consumer(() => new Consumer(_foo, _bar));

                ((IAmazonSqsReceiveEndpointConfigurator)x).Subscribe<Message>(m =>
                {
                    ((IAmazonSqsReceiveEndpointConfigurator)x).QueueSubscriptionAttributes["FilterPolicy"] = $"{{\"RoutingKey\": [\"{key}\"]}}";
                });
            });

            await handle.Ready;

            return handle;
        }


        class Consumer :
            IConsumer<Message>
        {
            readonly TaskCompletionSource<Message> _bar;
            readonly TaskCompletionSource<Message> _foo;

            public Consumer(TaskCompletionSource<Message> foo, TaskCompletionSource<Message> bar)
            {
                _foo = foo;
                _bar = bar;
            }

            public Task Consume(ConsumeContext<Message> context)
            {
                Console.WriteLine($"Received {context.Message.Value} for {context.Message.Key}");

                switch (context.Message.Key)
                {
                    case "foo":
                        _foo.TrySetResult(context.Message);
                        break;
                    case "bar":
                        _bar.TrySetResult(context.Message);
                        break;
                }

                return Task.CompletedTask;
            }
        }
    }
}
