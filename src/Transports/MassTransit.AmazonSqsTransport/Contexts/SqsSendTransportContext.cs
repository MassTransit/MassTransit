namespace MassTransit.AmazonSqsTransport.Contexts
{
    using Amazon.SQS.Model;
    using Context;
    using GreenPipes;
    using Transport;
    using Transports;


    public interface SqsSendTransportContext :
        SendTransportContext
    {
        IPipe<ClientContext> ConfigureTopologyPipe { get; }

        string EntityName { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }

        ITransportSetHeaderAdapter<MessageAttributeValue> SqsSetHeaderAdapter { get; }
        ITransportSetHeaderAdapter<Amazon.SimpleNotificationService.Model.MessageAttributeValue> SnsSetHeaderAdapter { get; }
    }
}
