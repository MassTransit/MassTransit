namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using Context;
    using GreenPipes;
    using Transport;


    public interface SqsSendTransportContext :
        SendTransportContext
    {
        IPipe<ClientContext> ConfigureTopologyPipe { get; }

        string EntityName { get; }

        Func<string, bool> CopyHeaderToMessageAttributesFilter { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }
    }
}
