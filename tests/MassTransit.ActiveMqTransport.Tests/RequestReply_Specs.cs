namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using Serialization;
    using TestFramework.Messages;


    [TestFixture]
    public class Request_reply_use_temporary_queue_name_envelope
        : ActiveMqTestFixture
    {
        [Test]
        public async Task Should_use_temporary_replyAddress()
        {
            var clientFactory = Bus.CreateClientFactory();
            RequestHandle<PingMessage> request = clientFactory.CreateRequest(new PingMessage());
            Response<PongMessage> response = await request.GetResponse<PongMessage>();

            TestExecutionContext.CurrentContext.OutWriter.Flush();

            Assert.IsNotNull(response);
            Assert.NotNull(_replyToAddress);
            Assert.IsTrue(_replyAddressPattern.IsMatch(_replyToAddress?.ToString()), "Reply address '{0}' does not match desired pattern", _replyToAddress);
        }

        Uri _replyToAddress;
        readonly Regex _replyAddressPattern = new Regex("ID:[^:]*:[^:]*:[^:]*", RegexOptions.Compiled);

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            TestTimeout = TimeSpan.FromMinutes(5);
            base.ConfigureActiveMqReceiveEndpoint(configurator);
            configurator.Handler<PingMessage>(async context =>
            {
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
            RequestHandle<PingMessage> request = clientFactory.CreateRequest(new PingMessage());
            Response<PongMessage> response = await request.GetResponse<PongMessage>();

            Assert.IsNotNull(response);
            Assert.NotNull(_replyToAddress);
            Assert.IsTrue(_replyAddressPattern.IsMatch(_replyToAddress?.ToString()), "Reply address '{0}' does not match desired pattern", _replyToAddress);
        }

        Uri _replyToAddress;
        readonly Regex _replyAddressPattern = new Regex("ID:[^:]*:[^:]*:[^:]*", RegexOptions.Compiled);

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureActiveMqReceiveEndpoint(configurator);
            configurator.Handler<PingMessage>(async context =>
            {
                _replyToAddress = context.ReceiveContext.TryGetPayload<ActiveMqReceiveContext>(out var payload)
                    ? payload.TransportMessage.NMSReplyTo.ToEndpointAddress()
                    : context.ResponseAddress;

                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            });
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            base.ConfigureActiveMqBus(configurator);

            configurator.UseRawJsonSerializer(RawSerializerOptions.AddTransportHeaders, true);
        }
    }
}
