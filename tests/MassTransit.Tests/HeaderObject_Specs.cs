﻿namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Serializing_an_object_in_a_header :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_serialize_and_deserialize()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                context.Headers.Set("Claims-Identity", new ClaimsIdentityProxy
                {
                    IdentityType = "AAD:Claims",
                    IdentityId = 27,
                    Claims = new[] { "One", "two", "Three" }
                });
            });

            ConsumeContext<PingMessage> consumeContext = await _handled;

            var identity = await _header.Task;

            Assert.Multiple(() =>
            {
                Assert.That(identity.IdentityId, Is.EqualTo(27));
                Assert.That(identity.IdentityType, Is.EqualTo("AAD:Claims"));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _handled;
        #pragma warning restore NUnit1032
        TaskCompletionSource<ClaimsIdentity> _header;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _header = GetTask<ClaimsIdentity>();

            _handled = Handler<PingMessage>(configurator, context =>
            {
                _header.TrySetResult(context.Headers.Get<ClaimsIdentity>("Claims-Identity"));

                return Task.CompletedTask;
            });
        }


        public interface ClaimsIdentity
        {
            string IdentityType { get; }
            int IdentityId { get; }
            string[] Claims { get; }
        }


        class ClaimsIdentityProxy :
            ClaimsIdentity
        {
            public string IdentityType { get; set; }
            public int IdentityId { get; set; }
            public string[] Claims { get; set; }
        }
    }
}
