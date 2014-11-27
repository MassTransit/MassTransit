// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Transports;
    using Util;


    public class SendRequestContext<TRequest> :
        RequestContext<TRequest>
        where TRequest : class
    {
        readonly IBus _bus;
        readonly List<HandlerHandle> _connections;
        readonly SendContext<TRequest> _context;
        readonly SynchronizationContext _currentSynchronizationContext;
        readonly Guid _requestId;
        readonly TaskCompletionSource<TRequest> _requestTask;
        SynchronizationContext _synchronizationContext;
        TimeSpan _timeout;

        public SendRequestContext(IBus bus, SendContext<TRequest> context, SynchronizationContext currentSynchronizationContext)
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (context == null)
                throw new ArgumentNullException("context");
            if (!context.RequestId.HasValue)
                throw new ArgumentException("The requestId must be initialized");

            _connections = new List<HandlerHandle>();

            _requestId = context.RequestId.Value;
            _bus = bus;
            _context = context;
            _currentSynchronizationContext = currentSynchronizationContext;

            _requestTask = new TaskCompletionSource<TRequest>(context.CancellationToken);

            HandleFault();
        }

        public Task<TRequest> Task
        {
            get { return _requestTask.Task; }
        }

        public Uri SourceAddress
        {
            get { return _context.SourceAddress; }
            set { _context.SourceAddress = value; }
        }

        public Uri DestinationAddress
        {
            get { return _context.DestinationAddress; }
            set { _context.DestinationAddress = value; }
        }

        public Uri ResponseAddress
        {
            get { return _context.ResponseAddress; }
            set { _context.ResponseAddress = value; }
        }

        public Uri FaultAddress
        {
            get { return _context.FaultAddress; }
            set { _context.FaultAddress = value; }
        }

        public Guid? RequestId
        {
            get { return _context.RequestId; }
            set { _context.RequestId = value; }
        }

        public Guid? MessageId
        {
            get { return _context.MessageId; }
            set { _context.MessageId = value; }
        }

        public Guid? CorrelationId
        {
            get { return _context.CorrelationId; }
            set { _context.CorrelationId = value; }
        }

        public SendContextHeaders ContextHeaders
        {
            get { return _context.ContextHeaders; }
        }

        public TimeSpan? TimeToLive
        {
            get { return _context.TimeToLive; }
            set { _context.TimeToLive = value; }
        }

        public ContentType ContentType
        {
            get { return _context.ContentType; }
            set { _context.ContentType = value; }
        }

        public bool Durable
        {
            get { return _context.Durable; }
            set { _context.Durable = value; }
        }

        public ISendMessageSerializer Serializer
        {
            get { return _context.Serializer; }
            set { _context.Serializer = value; }
        }

        public TRequest Message
        {
            get { return _context.Message; }
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload)
            where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public TimeSpan Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        public void UseCurrentSynchronizationContext()
        {
            _synchronizationContext = _currentSynchronizationContext;
        }

        public void SetSynchronizationContext(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        public void Watch<T>(MessageHandler<T> handler)
            where T : class
        {
            ConnectHandle connectHandle = _bus.SubscribeRequestHandler(_requestId, handler);

            _connections.Add(new HandlerHandle<T>(connectHandle));
        }

        public Task<T> Handle<T>(MessageHandler<T> handler)
            where T : class
        {
            var source = new TaskCompletionSource<T>();

            MessageHandler<T> messageHandler = async context =>
            {
                if (_synchronizationContext != null)
                {
                    _synchronizationContext.Post(state =>
                    {
                        try
                        {
                            handler(context).Wait(CancellationToken);

                            source.TrySetResult(context.Message);

                            Complete();
                        }
                        catch (Exception ex)
                        {
                            source.TrySetException(ex);

                            Fail(ex);
                        }
                    }, null);
                }
                else
                {
                    try
                    {
                        await handler(context);

                        source.TrySetResult(context.Message);

                        Complete();
                    }
                    catch (Exception ex)
                    {
                        source.TrySetException(ex);

                        Fail(ex);
                    }
                }
            };

            ConnectHandle connectHandle = _bus.SubscribeRequestHandler(_requestId, messageHandler);

            _connections.Add(new HandlerHandle<T>(connectHandle, source));

            return source.Task;
        }

        public Task<T> Handle<T>()
            where T : class
        {
            var source = new TaskCompletionSource<T>();

            MessageHandler<T> messageHandler = async context =>
            {
                try
                {
                    source.TrySetResult(context.Message);

                    Complete();
                }
                catch (Exception ex)
                {
                    source.TrySetException(ex);

                    Fail(ex);
                }
            };

            ConnectHandle connectHandle = _bus.SubscribeRequestHandler(_requestId, messageHandler);

            _connections.Add(new HandlerHandle<T>(connectHandle, source));

            return source.Task;
        }

        public Task<Fault<TRequest>> HandleFault()
        {
            var source = new TaskCompletionSource<Fault<TRequest>>();

            MessageHandler<Fault<TRequest>> messageHandler = async context =>
            {
                try
                {
                    Fail(context.Message);

                    source.TrySetResult(context.Message);
                }
                catch (Exception ex)
                {
                    source.TrySetException(ex);

                    Fail(ex);
                }
            };

            ConnectHandle connectHandle = _bus.SubscribeRequestHandler(_requestId, messageHandler);

            _connections.Add(new HandlerHandle<Fault<TRequest>>(connectHandle, source));

            return source.Task;
        }

        public void TimeoutExpired()
        {
            var timeoutException = new RequestTimeoutException(_requestId.ToString());

            _connections.ForEach(x =>
            {
                x.TrySetException(timeoutException);
                x.Disconnect();
            });

            _requestTask.TrySetException(timeoutException);
        }

        void Complete()
        {
            _connections.ForEach(x =>
            {
                x.TrySetCanceled();
                x.Disconnect();
            });

            _requestTask.TrySetResult(_context.Message);
        }

        void Fail(Fault<TRequest> fault)
        {
            Fail(new RequestFaultException(TypeMetadataCache<TRequest>.ShortName, fault, fault.Message));
        }

        void Fail(Exception ex)
        {
            _connections.ForEach(x =>
            {
                x.TrySetException(ex);
                x.Disconnect();
            });

            _requestTask.TrySetException(ex);
        }


        interface HandlerHandle :
            ConnectHandle
        {
            void TrySetException(Exception exception);
            void TrySetCanceled();
        }


        class HandlerHandle<T> :
            HandlerHandle
        {
            readonly ConnectHandle _handle;
            readonly TaskCompletionSource<T> _source;

            public HandlerHandle(ConnectHandle handle, TaskCompletionSource<T> source)
            {
                _handle = handle;
                _source = source;
            }

            public HandlerHandle(ConnectHandle handle)
            {
                _handle = handle;
                _source = new TaskCompletionSource<T>(TaskCreationOptions.None);
            }

            public void Dispose()
            {
                _handle.Dispose();
            }

            public void Disconnect()
            {
                _handle.Disconnect();
            }

            public void TrySetException(Exception exception)
            {
                _source.TrySetException(exception);
            }

            public void TrySetCanceled()
            {
                _source.TrySetCanceled();
            }
        }
    }
}