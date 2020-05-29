namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    public class When_a_message_deserialization_exception_occurs
        : InMemoryTestFixture
    {
        [Test]
        public async Task Should_put_message_in_error_queue()
        {
            await InputQueueSendEndpoint.Send(new BadMessage("Good"));


            //
            //            IEndpoint errorEndpoint =
            //                LocalBus.GetEndpoint(LocalBus.Endpoint.InboundTransport.Address.AppendToPath("_error"));
            //            errorEndpoint.InboundTransport.ShouldContain(errorEndpoint.Serializer, typeof(BadMessage));
            //
            //            LocalBus.Endpoint.ShouldNotContain<BadMessage>();
            //
            //            var errorTransport = LocalBus.Endpoint.ErrorTransport as LoopbackTransport;
            //            errorTransport.ShouldNotBe(null);
            //
            //            errorTransport.Count.ShouldBe(1);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handled<BadMessage>(configurator);
        }


        class BadMessage
        {
            public BadMessage()
            {
                throw new InvalidOperationException("I want to be bad.");
            }

            public BadMessage(string value)
            {
                Value = value;
            }

            public string Value { get; set; }
        }
    }
}
