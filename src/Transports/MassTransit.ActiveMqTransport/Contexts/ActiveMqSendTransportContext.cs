namespace MassTransit.ActiveMqTransport.Contexts
{
    using Apache.NMS;
    using Context;
    using GreenPipes;
    using Transport;


    public interface ActiveMqSendTransportContext :
        SendTransportContext
    {
        IPipe<SessionContext> ConfigureTopologyPipe { get; }

        string EntityName { get; }

        DestinationType DestinationType { get; }

        ISessionContextSupervisor SessionContextSupervisor { get; }
    }
}
