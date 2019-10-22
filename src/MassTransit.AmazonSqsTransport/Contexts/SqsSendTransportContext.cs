namespace MassTransit.AmazonSqsTransport.Contexts
{
    using Context;
    using GreenPipes;
    using Transport;


    public interface SqsSendTransportContext :
        SendTransportContext
    {
        IPipe<ClientContext> ConfigureTopologyPipe { get; }

        string EntityName { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }
    }
}
