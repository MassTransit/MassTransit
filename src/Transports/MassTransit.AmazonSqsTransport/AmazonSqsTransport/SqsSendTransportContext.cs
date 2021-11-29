namespace MassTransit.AmazonSqsTransport
{
    using Amazon.SQS.Model;
    using Transports;


    public interface SqsSendTransportContext :
        SendTransportContext,
        IPipeContextSource<ClientContext>
    {
        IPipe<ClientContext> ConfigureTopologyPipe { get; }

        string EntityName { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }

        ITransportSetHeaderAdapter<MessageAttributeValue> SqsSetHeaderAdapter { get; }
        ITransportSetHeaderAdapter<Amazon.SimpleNotificationService.Model.MessageAttributeValue> SnsSetHeaderAdapter { get; }
    }
}
