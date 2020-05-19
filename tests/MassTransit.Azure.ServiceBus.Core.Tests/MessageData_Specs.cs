namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Storage;
    using NUnit.Framework;
    using Storage;
    using Storage.MessageData;


    [TestFixture]
    public class Sending_a_message_with_data :
        AzureServiceBusTestFixture
    {
        Task<ConsumeContext<DataMessage>> _handler;
        readonly AzureStorageMessageDataRepository _repository;
        string _data;

        public Sending_a_message_with_data()
        {
            var account = CloudStorageAccount.Parse(Configuration.StorageAccount);
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

        [Test]
        public async Task Should_store_and_retrieve_the_message_data_from_blob_storage()
        {
            string data = NewId.NextGuid().ToString();

            var message = new DataMessage
            {
                CorrelationId = InVar.CorrelationId,
                Data = await _repository.PutString(data)
            };

            await InputQueueSendEndpoint.Send(message);

            var consumeContext = await _handler;

            Assert.That(consumeContext.Message.Data.HasValue, Is.True);

            Assert.That(_data, Is.EqualTo(data));
        }


        public class DataMessage
        {
            public Guid CorrelationId { get; set; }
            public MessageData<string> Data { get; set; }
        }
    }
}
