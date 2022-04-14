#nullable enable
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Clients;
    using Context;
    using DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Middleware;
    using Middleware.Outbox;
    using Serialization;
    using Transports;


    public class EntityFrameworkScopedBusContext<TBus, TDbContext> :
        ScopedBusContext,
        OutboxSendContext
        where TBus : class, IBus
        where TDbContext : DbContext
    {
        readonly TBus _bus;
        readonly IClientFactory _clientFactory;
        readonly TDbContext _dbContext;
        readonly Guid _outboxId;
        readonly IServiceProvider _provider;

        IPublishEndpoint? _publishEndpoint;
        IScopedClientFactory? _scopedClientFactory;
        ISendEndpointProvider? _sendEndpointProvider;

        public EntityFrameworkScopedBusContext(TBus bus, TDbContext dbContext, IClientFactory clientFactory, IServiceProvider provider)
        {
            _bus = bus;
            _dbContext = dbContext;
            _clientFactory = clientFactory;
            _provider = provider;

            _outboxId = NewId.NextGuid();
        }

        public async Task AddSend<T>(SendContext<T> context)
            where T : class
        {
            if (context.MessageId.HasValue == false)
                throw new MessageException(typeof(T), "The SendContext MessageId must be present");

            var body = context.Serializer.GetMessageBody(context);


            var now = DateTime.UtcNow;

            var outboxMessage = new OutboxMessage
            {
                MessageId = context.MessageId.Value,
                ConversationId = context.ConversationId,
                CorrelationId = context.CorrelationId,
                InitiatorId = context.InitiatorId,
                RequestId = context.RequestId,
                SourceAddress = context.SourceAddress,
                DestinationAddress = context.DestinationAddress,
                ResponseAddress = context.ResponseAddress,
                FaultAddress = context.FaultAddress,
                SentTime = context.SentTime ?? now,
                ContentType = context.ContentType?.ToString() ?? context.Serialization.DefaultContentType.ToString(),
                Body = body.GetString(),
                OutboxId = _outboxId
            };

            if (context.TimeToLive.HasValue)
                outboxMessage.ExpirationTime = now + context.TimeToLive;

            if (context.Delay.HasValue)
                outboxMessage.EnqueueTime = now + context.Delay;

            // TODO access to the object deserializer is needed here
            var headers = SystemTextJsonMessageSerializer.Instance.SerializeDictionary(context.Headers.GetAll());
            if (headers.Length > 0)
                outboxMessage.Headers = headers.GetString();

            if (context is TransportSendContext<T> transportSendContext)
            {
                var properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                transportSendContext.WritePropertiesTo(properties);
                if (properties.Count > 0)
                    outboxMessage.Properties = SystemTextJsonMessageSerializer.Instance.SerializeDictionary(properties).GetString();
            }

            await _dbContext.AddAsync(outboxMessage).ConfigureAwait(false);
        }

        public ISendEndpointProvider SendEndpointProvider
        {
            get { return _sendEndpointProvider ??= new ScopedSendEndpointProvider<IServiceProvider>(new OutboxSendEndpointProvider(this, _bus), _provider); }
        }

        public IPublishEndpoint PublishEndpoint
        {
            get
            {
                return _publishEndpoint ??= new PublishEndpoint(new ScopedPublishEndpointProvider<IServiceProvider>(
                    new OutboxPublishEndpointProvider(this, _bus), _provider));
            }
        }

        public IScopedClientFactory ClientFactory
        {
            get
            {
                return _scopedClientFactory ??=
                    new ScopedClientFactory(new ClientFactory(new ScopedClientFactoryContext<IServiceProvider>(_clientFactory, _provider)), null);
            }
        }
    }
}
