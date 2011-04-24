namespace MassTransit.Transports.ZeroMQ
{
    using System;

    public class ZeroMqTransportFactory :
        ITransportFactory
    {
        public string Scheme
        {
            get { return "zmq"; }
        }

        public ILoopbackTransport BuildLoopback(ITransportSettings settings)
        {
            return new ZeroMqTransport(settings.Address);
        }

        public IInboundTransport BuildInbound(ITransportSettings settings)
        {
            return BuildLoopback(settings);
        }

        public IOutboundTransport BuildOutbound(ITransportSettings settings)
        {
            return BuildLoopback(settings);
        }

        public IOutboundTransport BuildError(ITransportSettings settings)
        {
            return BuildLoopback(settings);
        }
    }
}