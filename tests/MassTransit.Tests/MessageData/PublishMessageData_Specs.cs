namespace MassTransit.Tests.MessageData
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.MessageData.Configuration;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Publishing_a_message_with_message_data :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_save_and_load_the_string()
        {
            await Bus.Publish<DocumentProcessed>(new
            {
                InVar.CorrelationId,
                StringData = "This is a huge string, and it is just too big to fit."
            });

            ConsumeContext<DocumentProcessed> completed = await _completed.Task;
        }

        TaskCompletionSource<ConsumeContext<DocumentProcessed>> _completed;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseMessageData(x => x.InMemory());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _completed = GetTask<ConsumeContext<DocumentProcessed>>();
            configurator.Handler<DocumentProcessed>(async context =>
            {
                if (!context.Message.StringData.HasValue)
                    throw new ArgumentException("StringData was not present.");

                var stringData = await context.Message.StringData.Value;
                if (string.IsNullOrWhiteSpace(stringData))
                    throw new ArgumentException("StringData was empty.");

                _completed.TrySetResult(context);
            });
        }


        public interface DocumentProcessed
        {
            MessageData<string> StringData { get; }
        }
    }
}
