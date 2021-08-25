using MassTransit.Configuration;
using MassTransit.Context;
using System;

namespace MassTransit.Transports.Outbox
{
    public class OnRampSendTransportContext :
        BaseSendTransportContext,
        IOnRampSendTransportContext
    {
        public OnRampSendTransportContext(IHostConfiguration hostConfiguration, Uri address, ISendTransport realSendTransport)
            : base(hostConfiguration)
        {
            DestinationAddress = address;
            RealSendTransport = realSendTransport;
        }

        public Uri DestinationAddress { get; }
        public ISendTransport RealSendTransport { get; } // rename BusSendTransport
    }

    public interface IOnRampSendTransportContext :
        SendTransportContext
    {
        Uri DestinationAddress { get; }
        ISendTransport RealSendTransport { get; } // rename BusSendTransport
    }
}
