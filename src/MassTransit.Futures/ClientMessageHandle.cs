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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ConsumeConfigurators;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Util;


    public abstract class ClientMessageHandle<TMessage> :
        Agent,
        MessageHandle<TMessage>,
        IPipe<SendContext<TMessage>>
        where TMessage : class
    {
        readonly CancellationToken _cancellationToken;
        readonly ClientFactoryContext _context;
        readonly Dictionary<Type, HandlerConnectHandle> _connections;
        readonly Uri _destinationAddress;
        readonly TMessage _message;
        readonly IBuildPipeConfigurator<SendContext<TMessage>> _pipeConfigurator;
        readonly Guid _requestId;
        readonly Task _send;
        readonly TaskCompletionSource<SendContext<TMessage>> _sendContext;
        readonly TaskScheduler _taskScheduler;
        readonly Timeout _timeout;
        CancellationTokenRegistration _registration;
        Timer _timeoutTimer;

        protected ClientMessageHandle(ClientFactoryContext context, Uri destinationAddress, TMessage message,
            CancellationToken cancellationToken = default, Timeout timeout = default, Guid? requestId = default, TaskScheduler taskScheduler = default)
        {
            _context = context;
            _destinationAddress = destinationAddress;
            _message = message;
            _cancellationToken = cancellationToken;

            _timeout = timeout.HasValue ? timeout : _context.DefaultTimeout.HasValue ? _context.DefaultTimeout.Value : Timeout.Default;

            _requestId = requestId ?? NewId.NextGuid();

            _taskScheduler = taskScheduler ??
                (SynchronizationContext.Current == null
                    ? TaskScheduler.Default
                    : TaskScheduler.FromCurrentSynchronizationContext());

            _pipeConfigurator = new PipeConfigurator<SendContext<TMessage>>();
            _sendContext = new TaskCompletionSource<SendContext<TMessage>>();
            _connections = new Dictionary<Type, HandlerConnectHandle>();

            _timeoutTimer = new Timer(TimeoutExpired, this, (long)_timeout.Value.TotalMilliseconds, -1L);

            if (cancellationToken != default && cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(Cancel);

            _send = SendRequest();

            HandleFault();
        }

        async Task IPipe<SendContext<TMessage>>.Send(SendContext<TMessage> context)
        {
            await Ready.ConfigureAwait(false);

            IPipe<SendContext<TMessage>> pipe = _pipeConfigurator.Build();

            context.RequestId = _requestId;
            context.ResponseAddress = _context.ResponseAddress;

            await pipe.Send(context).ConfigureAwait(false);

            _sendContext.TrySetResult(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        public Guid RequestId => _requestId;

        public void Cancel()
        {
            lock (_connections)
            {
                foreach (var connection in _connections.Values)
                    connection.TrySetCanceled();
            }
        }

        void IPipeConfigurator<SendContext<TMessage>>.AddPipeSpecification(IPipeSpecification<SendContext<TMessage>> specification)
        {
            _pipeConfigurator.AddPipeSpecification(specification);
        }

        public void Dispose()
        {
            _sendContext.TrySetCanceled();

            lock (_connections)
            {
                foreach (var handle in _connections.Values)
                    handle.Disconnect();
            }

            _timeoutTimer?.Dispose();
            _timeoutTimer = null;

            _registration.Dispose();
        }

        async Task SendRequest()
        {
            try
            {
                var sendEndpoint = await _context.GetSendEndpoint(_destinationAddress).ConfigureAwait(false);

                await sendEndpoint.Send(_message, this, _cancellationToken).ConfigureAwait(false);
            }
            catch (RequestException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new RequestException($"An exception occurred while processing the {typeof(TMessage).Name} request", exception);
            }
        }

        static SendContext FilterContext(SendContext<TMessage> context)
        {
            return context;
        }

        static SendContext<TMessage> MergeContext(SendContext<TMessage> input, SendContext context)
        {
            if (context is SendContext<TMessage> sendContext)
                return sendContext;

            return new SendContextProxy<TMessage>(context, input.Message);
        }

        protected Task<Result<T>> Result<T>(MessageHandler<T> handler = null, Action<IHandlerConfigurator<T>> configure = null)
            where T : class
        {
            lock (_connections)
            {
                if (_connections.ContainsKey(typeof(T)))
                    throw new RequestException($"Only one handler of type {TypeMetadataCache<T>.ShortName} can be registered");
            }

            var configurator = new ResultHandlerConfigurator<T>(_taskScheduler, handler, _send);

            configure?.Invoke(configurator);

            lock (_connections)
            {
                if (_cancellationToken.IsCancellationRequested)
                    return TaskUtil.Cancelled<Result<T>>();

                HandlerConnectHandle<T> handle = configurator.Connect(_context, _requestId);

                _connections.Add(typeof(T), handle);

                return handle.Task;
            }
        }

        void HandleFault()
        {
            Result<Fault<TMessage>>(FaultHandler);
        }

        Task FaultHandler(ConsumeContext<Fault<TMessage>> context)
        {
            Fail(context.Message);

            return TaskUtil.Completed;
        }

        void Fail(Fault message)
        {
            Fail(new RequestFaultException(TypeMetadataCache<TMessage>.ShortName, message));
        }

        void Fail(Exception exception)
        {
            lock (_connections)
            {
                foreach (var handle in _connections.Values)
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