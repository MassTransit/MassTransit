namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Configuration;
    using Transports;


    public class QueueSendTransportContext :
        BaseSendTransportContext,
        SendTransportContext<ClientContext>
    {
        readonly IPipe<ClientContext> _configureTopologyPipe;
        readonly ITransportSetHeaderAdapter<MessageAttributeValue> _headerAdapter;
        readonly IAmazonSqsHostConfiguration _hostConfiguration;
        readonly IClientContextSupervisor _supervisor;

        public QueueSendTransportContext(IAmazonSqsHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext,
            IClientContextSupervisor supervisor, IPipe<ClientContext> configureTopologyPipe, string entityName)
            : base(hostConfiguration, receiveEndpointContext.Serialization)
        {
            _hostConfiguration = hostConfiguration;
            _supervisor = supervisor;

            _configureTopologyPipe = configureTopologyPipe;
            EntityName = entityName;

            _headerAdapter = new TransportSetHeaderAdapter<MessageAttributeValue>(
                new SqsHeaderValueConverter(hostConfiguration.Settings.AllowTransportHeader), TransportHeaderOptions.IncludeFaultMessage);
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
            return [_supervisor];
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

            var message = new SendMessageBatchRequestEntry("", context.Body.GetString()) { Id = sendContext.MessageId.ToString() };

            _headerAdapter.Set(message.MessageAttributes, context.Headers);
            _headerAdapter.Set(message.MessageAttributes, MessageHeaders.ContentType, context.ContentType.ToString());
            _headerAdapter.Set(message.MessageAttributes, MessageHeaders.CorrelationId, context.CorrelationId);

            if (!string.IsNullOrEmpty(context.DeduplicationId))
                message.MessageDeduplicationId = context.DeduplicationId;

            if (!string.IsNullOrEmpty(context.GroupId))
                message.MessageGroupId = context.GroupId;

            var delay = context.Delay?.TotalSeconds;
            if (delay > 0)
                message.DelaySeconds = (int)Math.Min(delay.Value, 15 * 60);

            await transportContext.SendMessage(EntityName, message, sendContext.CancellationToken).ConfigureAwait(false);
        }
    }
}
