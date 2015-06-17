// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.NHibernateIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using MassTransit.Saga;
    using NHibernate;
    using Pipeline;
    using Util;


    public class NHibernateSagaConsumeContext<TSaga, TMessage> :
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get<NHibernateSagaRepository<TSaga>>();
        readonly ConsumeContext<TMessage> _context;
        readonly TSaga _instance;
        readonly ISession _session;
        bool _completed;
        readonly PayloadCache _payloadCache;

        public NHibernateSagaConsumeContext(ISession session, ConsumeContext<TMessage> context, TSaga instance)
        {
            _context = context;
            _instance = instance;
            _session = session;

            _payloadCache = new PayloadCache();
        }

        public Task CompleteTask
        {
            get { return _context.CompleteTask; }
        }

        public IEnumerable<string> SupportedMessageTypes
        {
            get { return _context.SupportedMessageTypes; }
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, publishPipe, cancellationToken);
        }

        public bool HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType) || _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            if (_payloadCache.TryGetPayload(out context))
                return true;

            return _context.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            TPayload payload;
            if (_payloadCache.TryGetPayload(out payload))
                return payload;

            if (_context.TryGetPayload(out payload))
                return payload;

            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        public Guid? MessageId
        {
            get { return _context.MessageId; }
        }

        public Guid? RequestId
        {
            get { return _context.RequestId; }
        }

        public Guid? CorrelationId
        {
            get { return _instance.CorrelationId; }
        }

        public DateTime? ExpirationTime
        {
            get { return _context.ExpirationTime; }
        }

        public Uri SourceAddress
        {
            get { return _context.SourceAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _context.DestinationAddress; }
        }

        public Uri ResponseAddress
        {
            get { return _context.ResponseAddress; }
        }

        public Uri FaultAddress
        {
            get { return _context.FaultAddress; }
        }

        public Headers Headers
        {
            get { return _context.Headers; }
        }

        public SagaConsumeContext<TSaga, T> PopContext<T>()
            where T : class
        {
            return (SagaConsumeContext<TSaga, T>)this;
        }

        async Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            _session.Delete(_instance);
            _completed = true;
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("SAGA:{0}:{1} Removed {2}", TypeMetadataCache<TSaga>.ShortName, _instance.CorrelationId,
                    TypeMetadataCache<TMessage>.ShortName);
            }
        }

        public bool IsCompleted
        {
            get { return _completed; }
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public ReceiveContext ReceiveContext
        {
            get { return _context.ReceiveContext; }
        }

        public bool HasMessageType(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class
        {
            return _context.TryGetMessage(out consumeContext);
        }

        public Task RespondAsync<T>(T message)
            where T : class
        {
            return _context.RespondAsync(message);
        }

        Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            return _context.RespondAsync(message, sendPipe);
        }

        public void Respond<T>(T message)
            where T : class
        {
            _context.Respond(message);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        void ConsumeContext.NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            _context.NotifyConsumed(context, duration, consumerType);
        }

        public void NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        TMessage ConsumeContext<TMessage>.Message
        {
            get { return _context.Message; }
        }

        public void NotifyConsumed(TimeSpan duration, string consumerType)
        {
            _context.NotifyConsumed(duration, consumerType);
        }

        public void NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            NotifyFaulted(_context, duration, consumerType, exception);
        }

        public TSaga Saga
        {
            get { return _instance; }
        }

        public ConnectHandle Connect(IPublishObserver observer)
        {
            return _context.Connect(observer);
        }
    }
}