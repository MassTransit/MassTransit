namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Collections.Generic;
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


    [TestFixture]
    public class Sending_a_message_with_object_data :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_store_and_retrieve_the_message_data_from_blob_storage()
        {
            var dataDictionary = new DataDictionary()
            {
                Values = new Dictionary<string, string>()
                {
                    { "First", "1st" },
                    { "Second", "2nd" }
                }
            };

            await InputQueueSendEndpoint.Send<DataMessage>(new
            {
                InVar.CorrelationId,
                Dictionary = dataDictionary
            });

            ConsumeContext<DataMessage> consumeContext = await _handler;

            Assert.That(consumeContext.Message.Dictionary.HasValue, Is.True);
            Assert.That(_data.Values, Is.EqualTo(dataDictionary.Values));
        }

        Task<ConsumeContext<DataMessage>> _handler;
        DataDictionary _data;
        IMessageDataRepository _repository;

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            _repository = configurator.UseMessageData(x => x.AzureStorage(Configuration.StorageAccount));
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handler<DataMessage>(configurator, async context =>
            {
                _data = await context.Message.Dictionary.Value;
            });
        }


        public class DataMessage
        {
            public Guid CorrelationId { get; set; }
            public MessageData<DataDictionary> Dictionary { get; set; }
        }


        public class DataDictionary
        {
            public Dictionary<string, string> Values { get; set; }
        }
    }


    [TestFixture]
    public class Sending_a_message_with_huge_object_data :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_store_and_retrieve_the_message_data_from_blob_storage()
        {
            var dataDictionary = new DataDictionary() { Values = new Dictionary<string, string>() };

            for (int i = 0; i < 10000; i++)
            {
                dataDictionary.Values.Add(i.ToString(), $"Value {i}");
            }

            await InputQueueSendEndpoint.Send<DataMessage>(new
            {
                InVar.CorrelationId,
                Dictionary = dataDictionary
            });

            ConsumeContext<DataMessage> consumeContext = await _handler;

            Assert.That(consumeContext.Message.Dictionary.HasValue, Is.True);
            Assert.That(_data.Values, Is.EqualTo(dataDictionary.Values));
        }

        Task<ConsumeContext<DataMessage>> _handler;
        DataDictionary _data;
        IMessageDataRepository _repository;

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            _repository = configurator.UseMessageData(x => x.AzureStorage(Configuration.StorageAccount));
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handler<DataMessage>(configurator, async context =>
            {
                _data = await context.Message.Dictionary.Value;
            });
        }


        public class DataMessage
        {
            public Guid CorrelationId { get; set; }
            public MessageData<DataDictionary> Dictionary { get; set; }
        }


        public class DataDictionary
        {
            public Dictionary<string, string> Values { get; set; }
        }
    }
}
