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
namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    using Util;


    public class RabbitMqConnectionCache :
        IConnectionCache,
        IProbeSite
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionCache>();
        readonly ConnectionFactory _connectionFactory;
        readonly object _scopeLock = new object();
        readonly RabbitMqHostSettings _settings;
        readonly TaskSupervisor _supervisor;
        readonly ITaskScope _taskScope;
        ConnectionScope _scope;

        public RabbitMqConnectionCache(RabbitMqHostSettings settings, TaskSupervisor supervisor)
        {
            _supervisor = supervisor;
            _settings = settings;
            _connectionFactory = settings.GetConnectionFactory();

            _taskScope = supervisor.CreateScope($"{TypeMetadataCache<RabbitMqConnectionCache>.ShortName} - {settings.ToDebugString()}", CloseScope);
        }

        public Task Send(IPipe<ConnectionContext> connectionPipe, CancellationToken cancellationToken)
        {
            ConnectionScope newScope = null;
            ConnectionScope existingScope;

            lock (_scopeLock)
            {
                existingScope = _scope;
                if (existingScope == null)
                {
                    newScope = new ConnectionScope(_taskScope);
                    _scope = newScope;
                }
            }
            if (existingScope != null)
                return SendUsingExistingConnection(connectionPipe, cancellationToken, existingScope);

            return SendUsingNewConnection(connectionPipe, newScope, cancellationToken);
        }

        public void Probe(ProbeContext context)
        {
            var connectionScope = _scope;
            if (connectionScope != null)
            {
                context.Set(new
                {
                    Connected = true
                });
            }
        }

        async Task CloseScope()
        {
            ConnectionScope existingScope;
            lock (_scopeLock)
            {
                existingScope = _scope;
            }

            if (existingScope != null)
                await existingScope.Close().ConfigureAwait(false);
        }

        async Task SendUsingNewConnection(IPipe<ConnectionContext> connectionPipe, ConnectionScope scope, CancellationToken cancellationToken)
        {
            try
            {
                if (_supervisor.StoppingToken.IsCancellationRequested)
                    throw new TaskCanceledException($"The connection is being disconnected: {_settings.ToDebugString()}");

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connecting: {0}", _connectionFactory.ToDebugString());

                var connection = _connectionFactory.CreateConnection();

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Connected: {0} (address: {1}, local: {2}", _connectionFactory.ToDebugString(),
                        connection.Endpoint, connection.LocalPort);
                }

                EventHandler<ShutdownEventArgs> connectionShutdown = null;
                connectionShutdown = (obj, reason) =>
                {
                    connection.ConnectionShutdown -= connectionShutdown;

                    Interlocked.CompareExchange(ref _scope, null, scope);

                    scope.Close();
                };

                connection.ConnectionShutdown += connectionShutdown;

                var connectionContext = new RabbitMqConnectionContext(connection, _settings, _taskScope);

                connectionContext.GetOrAddPayload(() => _settings);

                scope.Connected(connectionContext);
            }
            catch (BrokerUnreachableException ex)
            {
                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.ConnectFaulted(ex);

                throw new RabbitMqConnectionException("Connect failed: " + _connectionFactory.ToDebugString(), ex);
            }

            try
            {
                using (var context = await scope.Attach(cancellationToken).ConfigureAwait(false))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Using new connection: {0}", ((ConnectionContext)context).HostSettings.ToDebugString());

                    await connectionPipe.Send(context).ConfigureAwait(false);
                }
            }
            catch (BrokerUnreachableException ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The connection usage threw an exception", ex);

                Interlocked.CompareExchange(ref _scope, null, scope);

                throw new RabbitMqConnectionException("Connect failed: " + _connectionFactory.ToDebugString(), ex);
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The connection usage threw an exception", ex);

                Interlocked.CompareExchange(ref _scope, null, scope);

                throw;
            }
        }

        async Task SendUsingExistingConnection(IPipe<ConnectionContext> connectionPipe, CancellationToken cancellationToken, ConnectionScope scope)
        {
            try
            {
                using (var context = await scope.Attach(cancellationToken).ConfigureAwait(false))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Using existing connection: {0}", ((ConnectionContext)context).HostSettings.ToDebugString());

                    await connectionPipe.Send(context).ConfigureAwait(false);
                }
            }
            catch (BrokerUnreachableException ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The existing connection usage threw an exception", ex);

                Interlocked.CompareExchange(ref _scope, null, scope);

                throw new RabbitMqConnectionException("Connect failed: " + _connectionFactory.ToDebugString(), ex);
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The existing connection usage threw an exception", ex);

                Interlocked.CompareExchange(ref _scope, null, scope);

                throw;
            }
        }


        class ConnectionScope
        {
            static readonly ILog _log = Logger.Get<ConnectionScope>();
            readonly TaskCompletionSource<RabbitMqConnectionContext> _connectionContext;
            readonly ITaskScope _taskScope;
            bool _closed;

            public ConnectionScope(ITaskSupervisor supervisor)
            {
                _connectionContext = new TaskCompletionSource<RabbitMqConnectionContext>();

                _taskScope = supervisor.CreateScope("ConnectionScope", Close);
            }

            public void Connected(RabbitMqConnectionContext connectionContext)
            {
                _connectionContext.TrySetResult(connectionContext);

                _taskScope.SetReady();
            }

            public async Task<SharedConnectionContext> Attach(CancellationToken cancellationToken)
            {
                var connectionContext = await _connectionContext.Task.ConfigureAwait(false);

                var participant =
                    _taskScope.CreateParticipant($"{TypeMetadataCache<ConnectionScope>.ShortName} - {connectionContext.HostSettings.ToDebugString()}");

                return new SharedConnectionContext(connectionContext, cancellationToken, participant);
            }

            public async Task Close()
            {
                if (_closed)
                    return;

                var connectionContext = await _connectionContext.Task.ConfigureAwait(false);

                try
                {
                    using (var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60)))
                    {
                        await _taskScope.Stop("Closing connection", tokenSource.Token);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Close Faulted waiting for shared contexts", ex);
                }

                _closed = true;

                try
                {
                    connectionContext.Dispose();
                }
                catch (Exception exception)
                {
                    _log.Error("Close Faulted waiting for connection contexts", exception);
                }

                _taskScope.SetComplete();
            }

            public void ConnectFaulted(Exception exception)
            {
                _connectionContext.TrySetException(exception);

                _taskScope.SetNotReady(exception);

                _taskScope.SetComplete();
            }
        }
    }
}