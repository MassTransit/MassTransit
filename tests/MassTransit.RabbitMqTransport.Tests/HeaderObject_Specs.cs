namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Serializing_an_object_in_a_header :
        RabbitMqTestFixture
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
                    Claims = new[]
                    {
                        new ClaimProxy
                        {
                            Issuer = "Azure",
                            Type = "User",
                            Value = "37"
                        },
                        new ClaimProxy
                        {
                            Issuer = "Azure",
                            Type = "User",
                            Value = "457"
                        },
                        new ClaimProxy
                        {
                            Issuer = "Azure",
                            Type = "User",
                            Value = "451"
                        }
                    }
                });
            });

            ConsumeContext<PingMessage> consumeContext = await _handled;

            var identity = await _header.Task;

            Assert.AreEqual(27, identity.IdentityId);
            Assert.AreEqual("AAD:Claims", identity.IdentityType);
            Assert.AreEqual(3, identity.Claims.Length);
            Assert.AreEqual("Azure", identity.Claims[0].Issuer);
        }

        Task<ConsumeContext<PingMessage>> _handled;
        TaskCompletionSource<ClaimsIdentity> _header;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
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
            Claim[] Claims { get; }
        }


        public interface Claim
        {
            string Type { get; }
            string Value { get; }
            string ValueType { get; }
            string Issuer { get; }
        }


        public class ClaimProxy :
            Claim
        {
            public string Type { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
            public string Issuer { get; set; }
        }


        class ClaimsIdentityProxy :
            ClaimsIdentity
        {
            public string IdentityType { get; set; }
            public int IdentityId { get; set; }
            public Claim[] Claims { get; set; }
        }
    }


    [TestFixture]
    public class Header_properties_of_unsupported_values :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_support_date_time()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(), x =>
            {
                _now = DateTime.UtcNow;
                x.Headers.Set("Now", _now);

                _later = DateTimeOffset.Now;
                x.Headers.Set("Later", _later);
            });

            ConsumeContext<PingMessage> context = await _handled;

            Assert.AreEqual(_now, context.Headers.Get("Now", default(DateTime?)));

            Assert.AreEqual(_later, context.Headers.Get("Later", default(DateTimeOffset?)));
        }

        Task<ConsumeContext<PingMessage>> _handled;
        DateTime _now;
        DateTimeOffset _later;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _handled = Handled<PingMessage>(configurator);
        }
    }
}
