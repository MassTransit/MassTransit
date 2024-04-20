namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using TestFramework.Messages;


    [TestFixture]
    public class Request_reply_use_temporary_queue_name_envelope
        : ActiveMqTestFixture
    {
        [Test]
        public async Task Should_use_temporary_replyAddress()
        {
            var clientFactory = Bus.CreateClientFactory();
            RequestHandle<PingMessage> request = clientFactory.CreateRequest(new PingMessage(_pingId));
            Response<PongMessage> response = await request.GetResponse<PongMessage>();

            TestExecutionContext.CurrentContext.OutWriter.Flush();

            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(_replyToAddress, Is.Not.Null);
                Assert.That(_replyAddressPattern.IsMatch(_replyToAddress?.ToString()), Is.True,
                    $"Reply address '{_replyToAddress}' does not match desired pattern");
            });
        }

        Uri _replyToAddress;
        readonly Regex _replyAddressPattern = new Regex("ID:[^:]*:[^:]*:[^:]*", RegexOptions.Compiled);
        Guid _pingId;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            _pingId = NewId.NextGuid();

            TestTimeout = TimeSpan.FromMinutes(5);
            base.ConfigureActiveMqReceiveEndpoint(configurator);
            configurator.Handler<PingMessage>(async context =>
            {
                if (context.Message.CorrelationId != _pingId)
                    return;

                _replyToAddress = context.ReceiveContext.TryGetPayload<ActiveMqReceiveContext>(out var payload)
                    ? payload.TransportMessage.NMSReplyTo.ToEndpointAddress()
                    : context.ResponseAddress;

                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            });
        }
    }


    [TestFixture]
    public class Request_reply_use_temporary_queue_name_raw
        : ActiveMqTestFixture
    {
        [Test]
        public async Task Should_use_temporary_replyAddress()
        {
            var clientFactory = Bus.CreateClientFactory();
            RequestHandle<PingMessage> request = clientFactory.CreateRequest(new PingMessage(_pingId));
            Response<PongMessage> response = await request.GetResponse<PongMessage>();

            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(_replyToAddress, Is.Not.Null);
                Assert.That(_replyAddressPattern.IsMatch(_replyToAddress?.ToString()), Is.True,
                    $"Reply address '{_replyToAddress}' does not match desired pattern");
            });
        }

        Uri _replyToAddress;
        readonly Regex _replyAddressPattern = new Regex("ID:[^:]*:[^:]*:[^:]*", RegexOptions.Compiled);
        Guid _pingId;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            _pingId = NewId.NextGuid();

            base.ConfigureActiveMqReceiveEndpoint(configurator);
            configurator.Handler<PingMessage>(async context =>
            {
                if (context.Message.CorrelationId != _pingId)
                    return;

                _replyToAddress = context.ReceiveContext.TryGetPayload<ActiveMqReceiveContext>(out var payload)
                    ? payload.TransportMessage.NMSReplyTo.ToEndpointAddress()
                    : context.ResponseAddress;

                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            });
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            base.ConfigureActiveMqBus(configurator);

            configurator.UseRawJsonSerializer();
        }
    }
}
