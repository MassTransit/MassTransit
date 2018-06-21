// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ConsumeConfigurators;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Util;


    public class ClientRequestHandle<TRequest> :
        RequestHandle<TRequest>,
        IPipe<SendContext<TRequest>>
        where TRequest : class
    {
        readonly CancellationToken _cancellationToken;
        readonly ClientFactoryContext _context;
        readonly TRequest _message;
        readonly IBuildPipeConfigurator<SendContext<TRequest>> _pipeConfigurator;
        readonly TaskCompletionSource<bool> _readyToSend;
        readonly Guid _requestId;
        readonly IRequestSendEndpoint _requestSendEndpoint;
        readonly Dictionary<Type, HandlerConnectHandle> _responseHandlers;
        readonly Task _send;
        readonly TaskCompletionSource<SendContext<TRequest>> _sendContext;
        readonly TaskScheduler _taskScheduler;
        CancellationTokenRegistration _registration;
        Timer _timeoutTimer;
        RequestTimeout _timeToLive;

        public ClientRequestHandle(ClientFactoryContext context, IRequestSendEndpoint requestSendEndpoint, TRequest message,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default, Guid? requestId = default, TaskScheduler taskScheduler = default)
        {
            _context = context;
            _message = message;
            _requestSendEndpoint = requestSendEndpoint;
            _cancellationToken = cancellationToken;

            var requestTimeout = timeout.HasValue ? timeout : _context.DefaultTimeout.HasValue ? _context.DefaultTimeout.Value : RequestTimeout.Default;
            _timeToLive = requestTimeout;

            _requestId = requestId ?? NewId.NextGuid();

            _taskScheduler = taskScheduler ??
                (SynchronizationContext.Current == null
                    ? TaskScheduler.Default
                    : TaskScheduler.FromCurrentSynchronizationContext());

            _pipeConfigurator = new PipeConfigurator<SendContext<TRequest>>();
            _sendContext = new TaskCompletionSource<SendContext<TRequest>>();
            _readyToSend = new TaskCompletionSource<bool>();
            _responseHandlers = new Dictionary<Type, HandlerConnectHandle>();

            _timeoutTimer = new Timer(TimeoutExpired, this, (long)requestTimeout.Value.TotalMilliseconds, -1L);

            if (cancellationToken != default && cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(Cancel);

            _send = SendRequest();

            HandleFault();
        }

        async Task IPipe<SendContext<TRequest>>.Send(SendContext<TRequest> context)
        {
            await _readyToSend.Task.ConfigureAwait(false);

            context.RequestId = _requestId;
            context.ResponseAddress = _context.ResponseAddress;

            if (_timeToLive.HasValue)
                context.TimeToLive = _timeToLive.Value;

            IPipe<SendContext<TRequest>> pipe = _pipeConfigurator.Build();

            if (pipe.IsNotEmpty())
                await pipe.Send(context).ConfigureAwait(false);

            _sendContext.TrySetResult(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        Guid RequestHandle.RequestId => _requestId;

        RequestTimeout RequestHandle.TimeToLive
        {
            set => _timeToLive = value;
        }

        public void Cancel()
        {
            _readyToSend.TrySetCanceled();
            _sendContext.TrySetCanceled();

            lock (_responseHandlers)
            {
                foreach (var handler in _responseHandlers.Values)
                    handler.TrySetCanceled();
            }
        }

        void IPipeConfigurator<SendContext<TRequest>>.AddPipeSpecification(IPipeSpecification<SendContext<TRequest>> specification)
        {
            _pipeConfigurator.AddPipeSpecification(specification);
        }

        Task<Response<T>> RequestHandle.GetResponse<T>(bool readyToSend)
        {
            Task<Response<T>> response = Response<T>();

            if (readyToSend)
                _readyToSend.TrySetResult(true);

            return response;
        }

        public void Dispose()
        {
            Cancel();

            lock (_responseHandlers)
            {
                foreach (var handle in _responseHandlers.Values)
                    handle.Disconnect();
            }

            _timeoutTimer?.Dispose();
            _timeoutTimer = null;

            _registration.Dispose();
        }

        TRequest RequestHandle<TRequest>.Message => _message;

        async Task SendRequest()
        {
            try
            {
                await _requestSendEndpoint.Send(_message, this, _cancellationToken).ConfigureAwait(false);
            }
            catch (RequestException exception)
            {
                Fail(exception);

                throw;
            }
            catch (Exception exception)
            {
                Fail(exception);

                throw new RequestException($"An exception occurred while processing the {typeof(TRequest).Name} request", exception);
            }
        }

        Task<Response<T>> Response<T>(MessageHandler<T> handler = null, Action<IHandlerConfigurator<T>> configure = null)
            where T : class
        {
            lock (_responseHandlers)
            {
                if (_responseHandlers.ContainsKey(typeof(T)))
                    throw new RequestException($"Only one handler of type {TypeMetadataCache<T>.ShortName} can be registered");
            }

            var configurator = new ResponseHandlerConfigurator<T>(_taskScheduler, handler, _send);

            configure?.Invoke(configurator);

            lock (_responseHandlers)
            {
                if (_cancellationToken.IsCancellationRequested)
                    return TaskUtil.Cancelled<Response<T>>();

                HandlerConnectHandle<T> handle = configurator.Connect(_context, _requestId);

                _responseHandlers.Add(typeof(T), handle);

                return handle.Task;
            }
        }

        void HandleFault()
        {
            Response<Fault<TRequest>>(FaultHandler);
        }

        Task FaultHandler(ConsumeContext<Fault<TRequest>> context)
        {
            Fail(context.Message);

            return TaskUtil.Completed;
        }

        void Fail(Fault message)
        {
            Fail(new RequestFaultException(TypeMetadataCache<TRequest>.ShortName, message));
        }

        void Fail(Exception exception)
        {
            _readyToSend.TrySetException(exception);
            _sendContext.TrySetException(exception);

            lock (_responseHandlers)
            {
                foreach (var handle in _responseHandlers.Values)
                    handle.TrySetException(exception);
            }
        }

        void TimeoutExpired(object state)
        {
            _timeoutTimer?.Dispose();
            _timeoutTimer = null;

            var timeoutException = new RequestTimeoutException(_requestId.ToString());

            Fail(timeoutException);
        }
    }
}