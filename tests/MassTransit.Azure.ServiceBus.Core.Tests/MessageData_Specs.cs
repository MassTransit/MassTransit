namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using MessageData;
    using NUnit.Framework;


    [TestFixture]
    public class Sending_a_message_with_data :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_store_and_retrieve_the_message_data_from_blob_storage()
        {
            var data = NewId.NextGuid().ToString();

            var message = new DataMessage
            {
                CorrelationId = InVar.CorrelationId,
                Data = await _repository.PutString(data)
            };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<DataMessage> consumeContext = await _handler;

            Assert.That(consumeContext.Message.Data.HasValue, Is.True);

            Assert.That(_data, Is.EqualTo(data));
        }

        Task<ConsumeContext<DataMessage>> _handler;
        string _data;
        IMessageDataRepository _repository;

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            _repository = configurator.UseMessageData(x => x.AzureStorage(Configuration.StorageAccount));
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handler<DataMessage>(configurator, async context =>
            {
                _data = await context.Message.Data.Value;
            });
        }


        public class DataMessage
        {
            public Guid CorrelationId { get; set; }
            public MessageData<string> Data { get; set; }
        }
    }
}
