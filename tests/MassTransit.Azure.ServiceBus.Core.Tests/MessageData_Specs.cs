namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using global::Azure.Storage.Blobs;
    using NUnit.Framework;
    using Storage;
    using Storage.MessageData;


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
        readonly AzureStorageMessageDataRepository _repository;
        string _data;

        public Sending_a_message_with_data()
        {
            var account = new BlobServiceClient(Configuration.StorageAccount);
            _repository = account.CreateMessageDataRepository("message-data");
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.UseMessageData(_repository);
            configurator.ConnectBusObserver(_repository);
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
