namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class XmlHeaderBug_Specs :
        QuartzInMemoryTestFixture
    {
        Consumer _consumer;

        [Test]
        public async Task Should_not_get_junk_headers()
        {
            await Bus.Publish<IMyMessage>(new {Description = "hi!"});

            await _consumer.Completed;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseXmlSerializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new Consumer();
            configurator.Instance(_consumer);
        }


        public interface IMyMessage
        {
            string Description { get; }
        }


        class Consumer :
            IConsumer<IMyMessage>
        {
            public Task Completed => _source.Task;

            readonly TaskCompletionSource<ConsumeContext<IMyMessage>> _source = TaskUtil.GetTask<ConsumeContext<IMyMessage>>();

            public async Task Consume(ConsumeContext<IMyMessage> context)
            {
                if (context.Headers.TryGetHeader("MT-Redelivery-Count", out var value))
                {
                    if (context.Headers.TryGetHeader("#text", out _))
                    {
                        _source.TrySetException(new Exception("The bogus text header was present"));
                        return;
                    }

                    _source.TrySetResult(context);
                    return;
                }

                if (value == null)
                {
                    await context.Redeliver(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}
