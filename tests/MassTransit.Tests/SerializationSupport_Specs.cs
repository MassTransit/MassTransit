namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_using_mixed_serialization_types :
        InMemoryTestFixture
    {
        public When_using_mixed_serialization_types()
        {
            TestTimeout = TimeSpan.FromSeconds(5);
        }

        [Test]
        public async Task Should_be_able_to_read_xml_when_using_json()
        {
            _responseReceived = await ConnectPublishHandler<B>();

            await InputQueueSendEndpoint.Send(new A { Key = "Hello" });

            await _requestReceived;

            await _responseReceived;
        }

        Task<ConsumeContext<A>> _requestReceived;
        Task<ConsumeContext<B>> _responseReceived;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseXmlSerializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.SerializerContentType = SystemTextJsonMessageSerializer.JsonContentType;

            _requestReceived = Handler<A>(configurator, context => context.RespondAsync(new B()));
        }


        class A
        {
            public string Key { get; set; }
        }


        class B
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
