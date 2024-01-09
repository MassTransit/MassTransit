namespace MassTransit.SqlTransport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Transports;


    public class QueueSendTransportContext :
        BaseSendTransportContext,
        SendTransportContext<ClientContext>
    {
        readonly IPipe<ClientContext> _configureTopologyPipe;
        readonly ISqlHostConfiguration _hostConfiguration;
        readonly IClientContextSupervisor _supervisor;

        public QueueSendTransportContext(ISqlHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext,
            IClientContextSupervisor supervisor, IPipe<ClientContext> configureTopologyPipe, string entityName)
            : base(hostConfiguration, receiveEndpointContext.Serialization)
        {
            _hostConfiguration = hostConfiguration;
            _supervisor = supervisor;

            _configureTopologyPipe = configureTopologyPipe;
            EntityName = entityName;
        }

        public override string EntityName { get; }
        public override string ActivitySystem => "db";

        public void Probe(ProbeContext context)
        {
        }

        public override async Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var sendContext = new SqlMessageSendContext<T>(message, cancellationToken);

            await pipe.Send(sendContext).ConfigureAwait(false);

            CopyIncomingIdentifiersIfPresent(sendContext);

            return sendContext;
        }

        public override IEnumerable<IAgent> GetAgentHandles()
        {
            return new IAgent[] { };
        }

        public Task Send(IPipe<ClientContext> pipe, CancellationToken cancellationToken = default)
        {
            return _hostConfiguration.Retry(() => _supervisor.Send(pipe, cancellationToken), cancellationToken, _supervisor.SendStopping);
        }

        public Task<SendContext<T>> CreateSendContext<T>(ClientContext context, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            return CreateSendContext(message, pipe, cancellationToken);
        }

        public async Task Send<T>(ClientContext clientContext, SendContext<T> sendContext)
            where T : class
        {
            SqlMessageSendContext<T> context = sendContext as SqlMessageSendContext<T>
                ?? throw new ArgumentException("Invalid SendContext<T> type", nameof(sendContext));

            await _configureTopologyPipe.Send(clientContext).ConfigureAwait(false);

            sendContext.CancellationToken.ThrowIfCancellationRequested();

            if (Activity.Current?.IsAllDataRequested ?? false)
            {
                if (!string.IsNullOrWhiteSpace(context.RoutingKey))
                    Activity.Current.SetTag(nameof(context.RoutingKey), context.RoutingKey);
                if (!string.IsNullOrWhiteSpace(context.PartitionKey))
                    Activity.Current.SetTag(nameof(context.PartitionKey), context.PartitionKey);
            }

            await clientContext.Send(EntityName, context).ConfigureAwait(false);
        }

        static void CopyIncomingIdentifiersIfPresent<T>(SqlMessageSendContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<ConsumeContext>(out var consumeContext) && consumeContext.TryGetPayload<SqlMessageContext>(out var dbMessageContext))
            {
                if (context.PartitionKey == null && dbMessageContext.PartitionKey != null)
                    context.PartitionKey = dbMessageContext.PartitionKey;
            }
        }
    }
}
