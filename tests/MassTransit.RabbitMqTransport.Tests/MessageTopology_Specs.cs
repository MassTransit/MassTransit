namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Disabling_consume_topology_for_one_message :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_only_get_the_consumed_message()
        {
            await Bus.Publish(new MessageOne { Value = "Invalid" });
            await InputQueueSendEndpoint.Send(new MessageOne { Value = "Valid" });

            ConsumeContext<MessageOne> handled = await _handled;

            Assert.That(handled.Message.Value, Is.EqualTo("Valid"));
        }

        Task<ConsumeContext<MessageOne>> _handled;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureMessageTopology<MessageOne>(false);

            _handled = Handled<MessageOne>(configurator);
        }


        class MessageOne
        {
            public string Value { get; set; }
        }
    }
}
