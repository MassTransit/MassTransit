using MassTransit.Configuration;
using MassTransit.Context;
using System;

namespace MassTransit.Transports.Outbox
{
    public class OutboxSendTransportContext :
        BaseSendTransportContext,
        IOutboxSendTransportContext
    {
        public OutboxSendTransportContext(IHostConfiguration hostConfiguration, Uri address, ISendTransport realSendTransport)
            : base(hostConfiguration)
        {
            DestinationAddress = address;
            RealSendTransport = realSendTransport;
        }

        public Uri DestinationAddress { get; }
        public ISendTransport RealSendTransport { get; }
    }

    public interface IOutboxSendTransportContext :
        SendTransportContext
    {
        Uri DestinationAddress { get; }
        ISendTransport RealSendTransport { get; }
    }
}
