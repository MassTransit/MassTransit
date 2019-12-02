namespace MassTransit.AmazonSqsTransport.Contexts
{
    using Amazon.SQS.Model;
    using Context;
    using GreenPipes;
    using Transport;
    using Transports;


    public class HostSqsSendTransportContext :
        BaseSendTransportContext,
        SqsSendTransportContext
    {
        public HostSqsSendTransportContext(IClientContextSupervisor clientContextSupervisor, IPipe<ClientContext> configureTopologyPipe, string entityName,
            ILogContext logContext, AllowTransportHeader allowTransportHeader)
            : base(logContext)
        {
            ClientContextSupervisor = clientContextSupervisor;
            ConfigureTopologyPipe = configureTopologyPipe;
            EntityName = entityName;

            SqsSetHeaderAdapter = new TransportSetHeaderAdapter<MessageAttributeValue>(new SqsHeaderValueConverter(allowTransportHeader),
                TransportHeaderOptions.IncludeFaultMessage);
            SnsSetHeaderAdapter = new TransportSetHeaderAdapter<Amazon.SimpleNotificationService.Model.MessageAttributeValue>(
                new SnsHeaderValueConverter(allowTransportHeader), TransportHeaderOptions.IncludeFaultMessage);
        }

        public IPipe<ClientContext> ConfigureTopologyPipe { get; }
        public string EntityName { get; }
        public IClientContextSupervisor ClientContextSupervisor { get; }

        public ITransportSetHeaderAdapter<MessageAttributeValue> SqsSetHeaderAdapter { get; }
        public ITransportSetHeaderAdapter<Amazon.SimpleNotificationService.Model.MessageAttributeValue> SnsSetHeaderAdapter { get; }
    }
}
