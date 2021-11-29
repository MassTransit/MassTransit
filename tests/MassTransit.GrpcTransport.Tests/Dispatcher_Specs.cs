namespace MassTransit.GrpcTransport.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serialization;
    using TestFramework;
    using Testing;
    using Transports;


    [TestFixture]
    public class Dispatching_a_string :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_be_handled_by_the_consumer()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ILoggerFactory>(BusTestFixture.LoggerFactory);
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<SimpleCommandConsumer>();
                x.AddConsumer<SimpleEventConsumer>();

                x.UsingGrpc((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context, filter => filter.Include<SimpleEventConsumer>());
                });

                x.AddConfigureEndpointsCallback((name, cfg) => cfg.UseRawJsonSerializer());
            });

            await using var provider = services
                .BuildServiceProvider(true);

            await provider.GetRequiredService<IBusDepot>().Start(TestCancellationToken);
            try
            {
                var receiver = provider.GetRequiredService<IReceiveEndpointDispatcher<SimpleCommandConsumer>>();

                (var bytes, Dictionary<string, object> headers) = Serialize(new SimpleCommand { Value = "Hello" });

                await receiver.Dispatch(bytes, headers, TestCancellationToken);

                await SimpleEventConsumer.Completed.OrCanceled(TestCancellationToken);
            }
            finally
            {
                await provider.GetRequiredService<IBusDepot>().Stop(TestCancellationToken);
            }

            await SimpleEventConsumer.Completed.OrCanceled(TestCancellationToken);
        }

        static (byte[], Dictionary<string, object>) Serialize<T>(T obj)
            where T : class
        {
            var serializer = new NewtonsoftRawJsonMessageSerializer();

            var sendContext = new MessageSendContext<T>(obj);

            var bytes = serializer.GetMessageBody(sendContext).GetBytes();

            var headers = new Dictionary<string, object>
            {
                { MessageHeaders.ContentType, NewtonsoftRawJsonMessageSerializer.ContentTypeHeaderValue },
                { MessageHeaders.MessageId, sendContext.MessageId }
            };

            headers.Set(sendContext.Headers);

            return (bytes, headers);
        }

        public Dispatching_a_string()
            : base(new InMemoryTestHarness())
        {
        }


        class SimpleCommandConsumer :
            IConsumer<SimpleCommand>
        {
            public Task Consume(ConsumeContext<SimpleCommand> context)
            {
                return context.Publish(new SimpleEvent { Value = context.Message.Value });
            }
        }


        class SimpleEventConsumer :
            IConsumer<SimpleEvent>
        {
            static readonly TaskCompletionSource<ConsumeContext<SimpleEvent>> _source = new TaskCompletionSource<ConsumeContext<SimpleEvent>>();

            public static Task<ConsumeContext<SimpleEvent>> Completed => _source.Task;

            public Task Consume(ConsumeContext<SimpleEvent> context)
            {
                _source.TrySetResult(context);

                return Task.CompletedTask;
            }
        }


        class SimpleCommand
        {
            public string Value { get; set; }
        }


        class SimpleEvent
        {
            public string Value { get; set; }
        }
    }
}
