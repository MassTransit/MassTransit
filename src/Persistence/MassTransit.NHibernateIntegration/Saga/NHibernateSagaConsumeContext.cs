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
        readonly PayloadCache _payloadCache;
        readonly ISession _session;
        bool _completed;

        public NHibernateSagaConsumeContext(ISession session, ConsumeContext<TMessage> context, TSaga instance)
        {
            _context = context;
            _instance = instance;
            _session = session;

            _payloadCache = new PayloadCache();
        }

        Task ConsumeContext.CompleteTask => _context.CompleteTask;
        IEnumerable<string> ConsumeContext.SupportedMessageTypes => _context.SupportedMessageTypes;

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

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType) || _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload context)
        {
            if (_payloadCache.TryGetPayload(out context))
                return true;

            return _context.TryGetPayload(out context);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            TPayload payload;
            if (_payloadCache.TryGetPayload(out payload))
                return payload;

            if (_context.TryGetPayload(out payload))
                return payload;

            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        Guid? MessageContext.MessageId => _context.MessageId;
        Guid? MessageContext.RequestId => _context.RequestId;
        Guid? MessageContext.CorrelationId => _instance.CorrelationId;
        DateTime? MessageContext.ExpirationTime => _context.ExpirationTime;
        Uri MessageContext.SourceAddress => _context.SourceAddress;
        Uri MessageContext.DestinationAddress => _context.DestinationAddress;
        Uri MessageContext.ResponseAddress => _context.ResponseAddress;
        Uri MessageContext.FaultAddress => _context.FaultAddress;
        Headers MessageContext.Headers => _context.Headers;

        SagaConsumeContext<TSaga, T> SagaConsumeContext<TSaga>.PopContext<T>()
        {
            return (SagaConsumeContext<TSaga, T>)this;
        }

        Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            _session.Delete(_instance);
            _completed = true;
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("SAGA:{0}:{1} Removed {2}", TypeMetadataCache<TSaga>.ShortName, TypeMetadataCache<TMessage>.ShortName,
                    _instance.CorrelationId);
            }

            return TaskUtil.Completed;
        }

        bool SagaConsumeContext<TSaga>.IsCompleted => _completed;
        CancellationToken PipeContext.CancellationToken => _context.CancellationToken;
        ReceiveContext ConsumeContext.ReceiveContext => _context.ReceiveContext;

        bool ConsumeContext.HasMessageType(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        bool ConsumeContext.TryGetMessage<T>(out ConsumeContext<T> consumeContext)
        {
            return _context.TryGetMessage(out consumeContext);
        }

        Task ConsumeContext.RespondAsync<T>(T message)
        {
            return _context.RespondAsync(message);
        }

        Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            return _context.RespondAsync(message, sendPipe);
        }

        void ConsumeContext.Respond<T>(T message)
        {
            _context.Respond(message);
        }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        Task ConsumeContext.NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(context, duration, consumerType);
        }

        public Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            return _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        TMessage ConsumeContext<TMessage>.Message => _context.Message;

        Task ConsumeContext<TMessage>.NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(duration, consumerType);
        }

        Task ConsumeContext<TMessage>.NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(_context, duration, consumerType, exception);
        }

        public TSaga Saga => _instance;

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }
    }
}