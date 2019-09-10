namespace MassTransit.Tests.Initializers
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Specifying_a_message_header :
        InMemoryTestFixture
    {
        Task<ConsumeContext<Ping>> _handled;

        [Test]
        public async Task Should_set_the_header_on_send()
        {
            const string responseAddress = "loopback://localhost/client_queue";

            var requestId = NewId.NextGuid();
            DateTime now = DateTime.UtcNow;

            await Bus.Publish<Ping>(new
            {
                __ResponseAddress = responseAddress,
                __RequestId = requestId,
                __TimeToLive = 5000,
                __Header_Custom_Header_Value = "Frankie Say Relax",
                __Header_Custom_Header_Value2 = 27,
                Text = "Hello"
            });

            var context = await _handled;

            Assert.That(context.ResponseAddress, Is.EqualTo(new Uri(responseAddress)));
            Assert.That(context.RequestId, Is.EqualTo(requestId));
            Assert.That(context.ExpirationTime.HasValue, Is.True);
            Assert.That(context.ExpirationTime.Value, Is.GreaterThanOrEqualTo(now + TimeSpan.FromSeconds(5)));

            Assert.That(context.Headers.TryGetHeader("Custom-Header-Value", out object value), Is.True);
            Assert.That(value, Is.EqualTo("Frankie Say Relax"));

            Assert.That(context.Headers.TryGetHeader("Custom-Header-Value2", out value), Is.True);
            Assert.That(value, Is.EqualTo(27));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<Ping>(configurator);
        }


        public interface Ping
        {
            string Text { get; }
        }
    }
}
