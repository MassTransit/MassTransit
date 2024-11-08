namespace MassTransit.RabbitMqTransport
{
    using Transports;


    public class ChannelContextSupervisor :
        TransportPipeContextSupervisor<ChannelContext>,
        IChannelContextSupervisor
    {
        public ChannelContextSupervisor(IConnectionContextSupervisor connectionContextSupervisor, ushort? concurrentMessageLimit)
            : base(new ChannelContextFactory(connectionContextSupervisor, concurrentMessageLimit))
        {
            connectionContextSupervisor.AddConsumeAgent(this);
        }

        public ChannelContextSupervisor(IChannelContextSupervisor channelContextSupervisor)
            : base(new ScopeChannelContextFactory(channelContextSupervisor))
        {
            channelContextSupervisor.AddSendAgent(this);
        }
    }
}
