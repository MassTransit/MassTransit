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


    public class SendRequestContext<TRequest> :
        RequestContext<TRequest>
        where TRequest : class
    {
        readonly IBus _bus;
        readonly List<HandlerHandle> _connections;
        readonly SendContext<TRequest> _context;
        readonly Guid _requestId;
        readonly TaskCompletionSource<TRequest> _requestTask;
        TaskScheduler _taskScheduler;
        TimeSpan _timeout;
        CancellationTokenSource _timeoutToken;

        public SendRequestContext(IBus bus, SendContext<TRequest> context, TaskScheduler taskScheduler, Action<RequestContext<TRequest>> callback)
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (!context.RequestId.HasValue)
                throw new ArgumentException("The requestId must be initialized");

            _connections = new List<HandlerHandle>();

            _requestId = context.RequestId.Value;
            _bus = bus;
            _context = context;
            _taskScheduler = taskScheduler;

            _requestTask = new TaskCompletionSource<TRequest>(context.CancellationToken);

            HandleFault();

            callback(this);

            if (_timeout > TimeSpan.Zero)
            {
                _timeoutToken = new CancellationTokenSource(_timeout);
                _timeoutToken.Token.Register(TimeoutExpired);
            }
        }

        Task RequestContext.Task => _requestTask.Task;

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

        TimeSpan RequestContext.Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        void RequestContext.UseCurrentSynchronizationContext()
        {
            _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        void RequestContext.SetTaskScheduler(TaskScheduler taskScheduler)
        {
            _taskScheduler = taskScheduler;
        }

        void RequestContext.Watch<T>(MessageHandler<T> handler)
        {
            ConnectHandle connectHandle = _bus.ConnectRequestHandler(_requestId, handler);

            _connections.Add(new HandlerHandle<T>(connectHandle));
        }

        Task<T> RequestContext.Handle<T>(MessageHandler<T> handler)
        {
            var source = new TaskCompletionSource<T>();

            MessageHandler<T> messageHandler = async context =>
            {
                try
                {
                    await Task.Factory.StartNew(() => handler(context), context.CancellationToken, TaskCreationOptions.None, _taskScheduler);

                    source.TrySetResult(context.Message);

                    Complete();
                }
                catch (Exception ex)
                {
                    source.TrySetException(ex);

                    Fail(ex);
                }
            };

            ConnectHandle connectHandle = _bus.ConnectRequestHandler(_requestId, messageHandler);

            _connections.Add(new HandlerHandle<T>(connectHandle, source));

            return source.Task;
        }

        Task<T> RequestContext.Handle<T>()
        {
            var source = new TaskCompletionSource<T>();

            MessageHandler<T> messageHandler = context =>
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

                return TaskUtil.Completed;
            };

            ConnectHandle connectHandle = _bus.ConnectRequestHandler(_requestId, messageHandler);

            _connections.Add(new HandlerHandle<T>(connectHandle, source));

            return source.Task;
        }

        void HandleFault()
        {
            var source = new TaskCompletionSource<Fault<TRequest>>();

            MessageHandler<Fault<TRequest>> messageHandler = context =>
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

                return TaskUtil.Completed;
            };

            ConnectHandle connectHandle = _bus.ConnectRequestHandler(_requestId, messageHandler);

            _connections.Add(new HandlerHandle<Fault<TRequest>>(connectHandle, source));
        }

        void TimeoutExpired()
        {
            if (_timeoutToken != null)
            {
                _timeoutToken.Dispose();
                _timeoutToken = null;
            }

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
            if (_timeoutToken != null)
            {
                _timeoutToken.Dispose();
                _timeoutToken = null;
            }

            _connections.ForEach(x =>
            {
                x.TrySetCanceled();
                x.Disconnect();
            });

            _requestTask.TrySetResult(_context.Message);
        }

        void Fail(Fault<TRequest> fault)
        {
            Fail(new RequestFaultException(TypeMetadataCache<TRequest>.ShortName, fault));
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