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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public class PublishRequestContext<TRequest> :
        RequestContext<TRequest>
        where TRequest : class
    {
        readonly IDictionary<Type, RequestHandlerHandle> _connections;
        readonly SendContext<TRequest> _context;
        readonly Task<TRequest> _requestTask;

        public PublishRequestContext(IBus bus, SendContext<TRequest> context, Action<RequestContext<TRequest>> callback,
            IDictionary<Type, RequestHandlerHandle> connections, Task<TRequest> requestTask)
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (!context.RequestId.HasValue)
                throw new ArgumentException("The requestId must be initialized");

            _connections = connections;
            _requestTask = requestTask;
            _context = context;

            callback(this);
        }

        Task RequestContext.Task => _requestTask;

        Uri SendContext.SourceAddress
        {
            get { return _context.SourceAddress; }
            set { _context.SourceAddress = value; }
        }

        Uri SendContext.DestinationAddress
        {
            get { return _context.DestinationAddress; }
            set { _context.DestinationAddress = value; }
        }

        Uri SendContext.ResponseAddress
        {
            get { return _context.ResponseAddress; }
            set { _context.ResponseAddress = value; }
        }

        Uri SendContext.FaultAddress
        {
            get { return _context.FaultAddress; }
            set { _context.FaultAddress = value; }
        }

        Guid? SendContext.RequestId
        {
            get { return _context.RequestId; }
            set { _context.RequestId = value; }
        }

        Guid? SendContext.MessageId
        {
            get { return _context.MessageId; }
            set { _context.MessageId = value; }
        }

        Guid? SendContext.CorrelationId
        {
            get { return _context.CorrelationId; }
            set { _context.CorrelationId = value; }
        }

        public Guid? ConversationId
        {
            get { return _context.ConversationId; }
            set { _context.ConversationId = value; }
        }

        public Guid? InitiatorId
        {
            get { return _context.InitiatorId; }
            set { _context.InitiatorId = value; }
        }

        SendHeaders SendContext.Headers => _context.Headers;

        TimeSpan? SendContext.TimeToLive
        {
            get { return _context.TimeToLive; }
            set { _context.TimeToLive = value; }
        }

        ContentType SendContext.ContentType
        {
            get { return _context.ContentType; }
            set { _context.ContentType = value; }
        }

        bool SendContext.Durable
        {
            get { return _context.Durable; }
            set { _context.Durable = value; }
        }

        IMessageSerializer SendContext.Serializer
        {
            get { return _context.Serializer; }
            set { _context.Serializer = value; }
        }

        TRequest SendContext<TRequest>.Message => _context.Message;

        CancellationToken PipeContext.CancellationToken => _context.CancellationToken;

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        TimeSpan RequestContext.Timeout { get; set; }

        public Task<TRequest> Task => _requestTask;

        void RequestContext.UseCurrentSynchronizationContext()
        {
        }

        void RequestContext.SetTaskScheduler(TaskScheduler taskScheduler)
        {
        }

        void RequestContext.Watch<T>(MessageHandler<T> handler)
        {
            RequestHandlerHandle connection;
            if (!_connections.TryGetValue(typeof(T), out connection))
                throw new RequestException($"The previously registered handler of type {TypeMetadataCache<T>.ShortName} was not registered");
        }

        Task<T> RequestContext.Handle<T>(MessageHandler<T> handler)
        {
            RequestHandlerHandle connection;
            if (!_connections.TryGetValue(typeof(T), out connection))
                throw new RequestException($"The previously registered handler of type {TypeMetadataCache<T>.ShortName} was not registered");

            return connection.GetTask<T>();
        }

        Task<T> RequestContext.Handle<T>()
        {
            RequestHandlerHandle connection;
            if (!_connections.TryGetValue(typeof(T), out connection))
                throw new RequestException($"The previously registered handler of type {TypeMetadataCache<T>.ShortName} was not registered");

            return connection.GetTask<T>();
        }
    }
}