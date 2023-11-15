#nullable enable
namespace MassTransit.ActiveMqTransport
{
    using System;
    using Apache.NMS;
    using Transports;


    public class ReplyToSendEndpoint :
        SendEndpointProxy
    {
        readonly IDestination _destination;

        public ReplyToSendEndpoint(ISendEndpoint endpoint, IDestination destination)
            : base(endpoint)
        {
            _destination = destination;
        }

        protected override IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>>? pipe = default)
        {
            return new ReplyToPipe<T>(_destination, pipe);
        }


        class ReplyToPipe<TMessage> :
            SendContextPipeAdapter<TMessage>
            where TMessage : class
        {
            readonly IDestination _destination;

            public ReplyToPipe(IDestination destination, IPipe<SendContext<TMessage>>? pipe)
                : base(pipe)
            {
                _destination = destination;
            }

            protected override void Send(SendContext<TMessage> context)
            {
                if (!context.TryGetPayload(out ConsumeContext? consumeContext))
                    return;

                if (!context.TryGetPayload(out ActiveMqSendContext? sendContext))
                    throw new ArgumentException("The ActiveMqSendContext was not available");

                if (string.Equals(context.DestinationAddress?.AbsolutePath, consumeContext.ResponseAddress?.AbsolutePath))
                    sendContext.ReplyDestination = _destination;
            }

            protected override void Send<T>(SendContext<T> context)
            {
            }
        }
    }
}
