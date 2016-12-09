// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using ConsumeConfigurators;
    using GreenPipes;
    using Pipeline;
    using Util;


    public class SendRequestConfigurator<TRequest> : 
        IRequestConfigurator<TRequest>
        where TRequest : class
    {
        readonly Dictionary<Type, RequestHandlerHandle> _connections;
        readonly IRequestPipeConnector _connector;
        readonly SendContext<TRequest> _context;
        readonly Guid _requestId;
        readonly TaskCompletionSource<TRequest> _requestTask;
        TaskScheduler _taskScheduler;
        CancellationTokenSource _timeoutToken;

        public SendRequestConfigurator(IRequestPipeConnector connector, SendContext<TRequest> context, TaskScheduler taskScheduler,
            Action<IRequestConfigurator<TRequest>> callback)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (!context.RequestId.HasValue)
                throw new ArgumentException("The requestId must be initialized");

            _connections = new Dictionary<Type, RequestHandlerHandle>();

            _requestId = context.RequestId.Value;
            _connector = connector;
            _context = context;
            _taskScheduler = taskScheduler;

            _requestTask = new TaskCompletionSource<TRequest>(context.CancellationToken);

            HandleFault();

            callback(this);

            if (Timeout > TimeSpan.Zero)
            {
                _timeoutToken = new CancellationTokenSource(Timeout);
                _timeoutToken.Token.Register(TimeoutExpired);
            }
        }

        public IDictionary<Type, RequestHandlerHandle> Connections => _connections;

        Task<TRequest> IRequestConfigurator<TRequest>.Task => _requestTask.Task;

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

        public Guid? ScheduledMessageId
        {
            get { return _context.ScheduledMessageId; }
            set { _context.ScheduledMessageId = value; }
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

        SendContext<T> SendContext.CreateProxy<T>(T message)
        {
            return _context.CreateProxy(message);
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

        public TimeSpan Timeout { get; set; }

        void IRequestConfigurator<TRequest>.UseCurrentSynchronizationContext()
        {
            _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        void IRequestConfigurator<TRequest>.SetTaskScheduler(TaskScheduler taskScheduler)
        {
            _taskScheduler = taskScheduler;
        }

        void IRequestConfigurator<TRequest>.Watch<T>(MessageHandler<T> handler, Action<IHandlerConfigurator<T>> configure)
        {
            if (_connections.ContainsKey(typeof(T)))
                throw new RequestException($"Only one handler of type {TypeMetadataCache<T>.ShortName} can be registered");

            var configurator = new RequestHandlerConfigurator<T>(handler);

            configure?.Invoke(configurator);

            var connectHandle = configurator.Connect(_connector, _requestId);

            var source = new TaskCompletionSource<T>();

            _connections.Add(typeof(T), new RequestHandlerHandle<T>(connectHandle, source, _context.CancellationToken));
        }

        Task<T> IRequestConfigurator<TRequest>.Handle<T>(MessageHandler<T> handler, Action<IHandlerConfigurator<T>> configure)
        {
            if (_connections.ContainsKey(typeof(T)))
                throw new RequestException($"Only one handler of type {TypeMetadataCache<T>.ShortName} can be registered");

            var source = new TaskCompletionSource<T>();

            MessageHandler<T> messageHandler = async context =>
            {
                try
                {
                    await Task.Factory.StartNew(() => handler(context), context.CancellationToken, TaskCreationOptions.None, _taskScheduler)
                        .Unwrap()
                        .ConfigureAwait(false);

                    source.TrySetResult(context.Message);

                    Complete();
                }
                catch (Exception ex)
                {
                    source.TrySetException(ex);

                    Fail(ex);
                }
            };

            var configurator = new RequestHandlerConfigurator<T>(messageHandler);

            configure?.Invoke(configurator);

            var connectHandle = configurator.Connect(_connector, _requestId);

            _connections.Add(typeof(T), new RequestHandlerHandle<T>(connectHandle, source, _context.CancellationToken));

            return source.Task;
        }

        Task<T> IRequestConfigurator<TRequest>.Handle<T>(Action<IHandlerConfigurator<T>> configure)
        {
            if (_connections.ContainsKey(typeof(T)))
                throw new RequestException($"Only one handler of type {TypeMetadataCache<T>.ShortName} can be registered");

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

            var configurator = new RequestHandlerConfigurator<T>(messageHandler);

            configure?.Invoke(configurator);

            var connectHandle = configurator.Connect(_connector, _requestId);

            _connections.Add(typeof(T), new RequestHandlerHandle<T>(connectHandle, source, _context.CancellationToken));

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

            var connectHandle = _connector.ConnectRequestHandler(_requestId, messageHandler);

            _connections.Add(typeof(Fault<TRequest>), new RequestHandlerHandle<Fault<TRequest>>(connectHandle, source, _context.CancellationToken));
        }

        void TimeoutExpired()
        {
            if (_timeoutToken != null)
            {
                _timeoutToken.Dispose();
                _timeoutToken = null;
            }

            var timeoutException = new RequestTimeoutException(_requestId.ToString());

            foreach (var handle in _connections.Values)
            {
                handle.TrySetException(timeoutException);
                handle.Disconnect();
            }

            _requestTask.TrySetException(timeoutException);
        }

        void Complete()
        {
            if (_timeoutToken != null)
            {
                _timeoutToken.Dispose();
                _timeoutToken = null;
            }

            foreach (var handle in _connections.Values)
            {
                handle.TrySetCanceled();
                handle.Disconnect();
            }

            _requestTask.TrySetResult(_context.Message);
        }

        void Fail(Fault<TRequest> fault)
        {
            Fail(new RequestFaultException(TypeMetadataCache<TRequest>.ShortName, fault));
        }

        void Fail(Exception ex)
        {
            foreach (var handle in _connections.Values)
            {
                handle.TrySetException(ex);
                handle.Disconnect();
            }

            _requestTask.TrySetException(ex);
        }


        /// <summary>
        /// Connects a handler to the inbound pipe of the receive endpoint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class RequestHandlerConfigurator<T> :
            IHandlerConfigurator<T>
            where T : class
        {
            readonly MessageHandler<T> _handler;
            readonly IList<IPipeSpecification<ConsumeContext<T>>> _specifications;

            public RequestHandlerConfigurator(MessageHandler<T> handler)
            {
                _handler = handler;
                _specifications = new List<IPipeSpecification<ConsumeContext<T>>>();
            }

            public void AddPipeSpecification(IPipeSpecification<ConsumeContext<T>> specification)
            {
                _specifications.Add(specification);
            }

            public ConnectHandle Connect(IRequestPipeConnector connector, Guid requestId)
            {
                var connectHandle = connector.ConnectRequestHandler(requestId, _handler, _specifications.ToArray());

                return connectHandle;
            }
        }


        class RequestHandlerHandle<TResponse> :
            RequestHandlerHandle
        {
            readonly ConnectHandle _handle;
            readonly TaskCompletionSource<TResponse> _source;
            CancellationTokenRegistration _registration;

            public RequestHandlerHandle(ConnectHandle handle, TaskCompletionSource<TResponse> source, CancellationToken cancellationToken)
            {
                _handle = handle;
                _source = source;
                _registration = cancellationToken.Register(TrySetCanceled);
            }

            public void Dispose()
            {
                _registration.Dispose();
                _handle.Dispose();
            }

            public void Disconnect()
            {
                _registration.Dispose();
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

            public Task<T> GetTask<T>()
            {
                return _source.Task as Task<T>;
            }
        }
    }
}