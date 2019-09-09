namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Initializers;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Sending_a_message_via_proxy
    {
        [Test]
        public async Task Should_property_initialize_the_values()
        {
            var values = new
            {
                CorrelationId = Guid.NewGuid(),
                Name = "Dru",
                Timestamp = DateTime.UtcNow,
            };

            InitializeContext<IMessageType> context = await MessageInitializerCache<IMessageType>.Initialize(values);

            var message = context.Message;
            message.CorrelationId.ShouldBe(values.CorrelationId);
            message.Name.ShouldBe(values.Name);
            message.Timestamp.ShouldBe(values.Timestamp);
        }


        public interface IMessageType
        {
            Guid CorrelationId { get; }
            string Name { get; }
            DateTime Timestamp { get; }
        }
    }
}
