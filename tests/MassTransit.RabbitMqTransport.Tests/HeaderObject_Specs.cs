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

            Assert.Multiple(() =>
            {
                Assert.That(identity.IdentityId, Is.EqualTo(27));
                Assert.That(identity.IdentityType, Is.EqualTo("AAD:Claims"));
                Assert.That(identity.Claims, Has.Length.EqualTo(3));
            });
            Assert.That(identity.Claims[0].Issuer, Is.EqualTo("Azure"));
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
                _sometime = DateTime.Now;
                x.Headers.Set("Sometime", _sometime);

                _now = DateTime.UtcNow;
                x.Headers.Set("Now", _now);

                _later = DateTimeOffset.Now;
                x.Headers.Set("Later", _later);
            });

            ConsumeContext<PingMessage> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.Headers.Get("Now", default(DateTime?)), Is.EqualTo(_now));

                Assert.That(context.Headers.Get("Later", default(DateTimeOffset?)), Is.EqualTo(_later));

                Assert.That(context.Headers.Get("Sometime", default(DateTime?)), Is.EqualTo(_sometime));
            });
        }

        Task<ConsumeContext<PingMessage>> _handled;
        DateTime _now;
        DateTimeOffset _later;
        DateTime _sometime;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _handled = Handled<PingMessage>(configurator);
        }
    }
}
