namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService.Model;
    using Configuration;
    using Transports;


    public class TopicSendTransportContext :
        BaseSendTransportContext,
        SendTransportContext<ClientContext>
    {
        readonly IPipe<ClientContext> _configureTopologyPipe;
        readonly ITransportSetHeaderAdapter<MessageAttributeValue> _headerAdapter;
        readonly IAmazonSqsHostConfiguration _hostConfiguration;
        readonly IClientContextSupervisor _supervisor;

        public TopicSendTransportContext(IAmazonSqsHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext,
            IClientContextSupervisor supervisor, IPipe<ClientContext> configureTopologyPipe, string entityName)
            : base(hostConfiguration, receiveEndpointContext.Serialization)
        {
            _hostConfiguration = hostConfiguration;
            _supervisor = supervisor;
            _configureTopologyPipe = configureTopologyPipe;

            EntityName = entityName;

            _headerAdapter = new TransportSetHeaderAdapter<MessageAttributeValue>(
                new SnsHeaderValueConverter(hostConfiguration.Settings.AllowTransportHeader), TransportHeaderOptions.IncludeFaultMessage);
        }

        public override string EntityName { get; }
        public override string ActivitySystem => "aws_sqs";

        public Task Send(IPipe<ClientContext> pipe, CancellationToken cancellationToken = default)
        {
            return _hostConfiguration.Retry(() => _supervisor.Send(pipe, cancellationToken), cancellationToken, _supervisor.SendStopping);
        }

        public void Probe(ProbeContext context)
        {
            _supervisor.Probe(context);
        }

        public override async Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var sendContext = new AmazonSqsMessageSendContext<T>(message, cancellationToken);

            await pipe.Send(sendContext).ConfigureAwait(false);

            return sendContext;
        }

        public override IEnumerable<IAgent> GetAgentHandles()
        {
            return new IAgent[] { _supervisor };
        }

        public Task<SendContext<T>> CreateSendContext<T>(ClientContext context, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return CreateSendContext(message, pipe, cancellationToken);
        }

        public async Task Send<T>(ClientContext transportContext, SendContext<T> sendContext)
            where T : class
        {
            AmazonSqsMessageSendContext<T> context = sendContext as AmazonSqsMessageSendContext<T>
                ?? throw new ArgumentException("Invalid SendContext<T> type", nameof(sendContext));

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            await _configureTopologyPipe.Send(transportContext).ConfigureAwait(false);

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            var request = new PublishBatchRequestEntry { Message = context.Body.GetString() };

            _headerAdapter.Set(request.MessageAttributes, context.Headers);
            _headerAdapter.Set(request.MessageAttributes, MessageHeaders.ContentType, context.ContentType.ToString());
            _headerAdapter.Set(request.MessageAttributes, nameof(context.CorrelationId), context.CorrelationId);

            if (!string.IsNullOrEmpty(context.DeduplicationId))
                request.MessageDeduplicationId = context.DeduplicationId;

            if (!string.IsNullOrEmpty(context.GroupId))
                request.MessageGroupId = context.GroupId;

            await transportContext.Publish(EntityName, request, context.CancellationToken).ConfigureAwait(false);
        }
    }
}
