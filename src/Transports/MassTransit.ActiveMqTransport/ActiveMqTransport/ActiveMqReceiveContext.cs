namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Transports;


    public sealed class ActiveMqReceiveContext :
        BaseReceiveContext,
        ActiveMqMessageContext,
        ReceiveLockContext
    {
        public ActiveMqReceiveContext(IMessage transportMessage, ActiveMqReceiveEndpointContext context, params object[] payloads)
            : base(transportMessage.NMSRedelivered, context, payloads)
        {
            TransportMessage = transportMessage;

            Body = new ActiveMqMessageBody(transportMessage);
        }

        protected override IHeaderProvider HeaderProvider => new ActiveMqHeaderProvider(TransportMessage);

        public override MessageBody Body { get; }

        public IMessage TransportMessage { get; }

        public IPrimitiveMap Properties => TransportMessage.Properties;

        public Task Complete()
        {
            TransportMessage.Acknowledge();

            return Task.CompletedTask;
        }

        public Task Faulted(Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task ValidateLockStatus()
        {
            return Task.CompletedTask;
        }
    }
}
