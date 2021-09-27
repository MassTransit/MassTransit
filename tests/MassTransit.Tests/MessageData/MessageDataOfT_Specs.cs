namespace MassTransit.Tests.MessageData
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using MassTransit.MessageData;
    using NUnit.Framework;
    using TestFramework;


    public class Sending_message_data_with_an_object_type :
        InMemoryTestFixture
    {
        readonly IMessageDataRepository _repository = new InMemoryMessageDataRepository();
        Uri _payloadAddress;

        [Test]
        public async Task Should_load_the_data_from_the_repository()
        {
            IRequestClient<ProcessPayload> client = CreateRequestClient<ProcessPayload>();

            var payload = new SpecialPayload { Value = "Something special" };

            var streamBytes = new byte[1000];
            await using var ms = new MemoryStream(streamBytes);

            Response<PayloadProcessed> response = await client.GetResponse<PayloadProcessed>(new { Payload = payload }, TestCancellationToken,
                RequestTimeout.After(s: 5));

            Assert.That(response.Message.Payload, Is.Not.Null);
            Assert.That(response.Message.Payload.HasValue, Is.True);
            Assert.That(response.Message.Payload.Address, Is.EqualTo(_payloadAddress), "Should use the existing message data address");
            var responsePayload = await response.Message.Payload.Value;
            Assert.That(responsePayload.Value, Is.EqualTo(payload.Value));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseMessageData(_repository);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<ProcessPayload>(async context =>
            {
                if (!context.Message.Payload.HasValue)
                    throw new ArgumentException("Payload was not present.");

                var payload = await context.Message.Payload.Value;
                if (payload == null)
                    throw new ArgumentException("Payload was null.");

                _payloadAddress = context.Message.Payload.Address;

                await context.RespondAsync<PayloadProcessed>(new
                {
                    context.Message.Payload,
                });
            });
        }


        public class SpecialPayload
        {
            public string Value { get; set; }
        }


        public interface ProcessPayload
        {
            MessageData<SpecialPayload> Payload { get; }
        }


        public interface PayloadProcessed
        {
            MessageData<SpecialPayload> Payload { get; }
        }
    }
}
