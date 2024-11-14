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
        [Test]
        public async Task Should_set_the_header_on_send()
        {
            const string responseAddress = "loopback://localhost/client_queue";

            var requestId = NewId.NextGuid();
            var now = DateTime.UtcNow;

            await Bus.Publish<Ping>(new
            {
                __ResponseAddress = responseAddress,
                __RequestId = requestId,
                __TimeToLive = 5000,
                __Header_Custom_Header_Value = "Frankie Say Relax",
                __Header_Custom_Header_Value2 = 27,
                Text = "Hello"
            });

            ConsumeContext<Ping> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.ResponseAddress, Is.EqualTo(new Uri(responseAddress)));
                Assert.That(context.RequestId, Is.EqualTo(requestId));
                Assert.That(context.ExpirationTime.HasValue, Is.True);
                Assert.That(context.ExpirationTime.Value, Is.GreaterThanOrEqualTo(now + TimeSpan.FromSeconds(5)));
            });
            Assert.Multiple(() =>
            {
                Assert.That(context.Headers.TryGetHeader("Custom-Header-Value", out var value), Is.True);
                Assert.That(value, Is.EqualTo("Frankie Say Relax"));
            });
            Assert.Multiple(() =>
            {
                Assert.That(context.Headers.TryGetHeader("Custom-Header-Value2", out var value), Is.True);
                Assert.That(value, Is.EqualTo(27));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<Ping>> _handled;
        #pragma warning restore NUnit1032

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
