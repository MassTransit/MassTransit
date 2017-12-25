// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;
    using Topology;
    using Util;


    public class RabbitMqConnectionCache :
        IConnectionCache,
        IProbeSite
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionCache>();
        readonly ITaskScope _cacheTaskScope;
        readonly Lazy<ConnectionFactory> _connectionFactory;
        readonly object _scopeLock = new object();
        readonly RabbitMqHostSettings _settings;
        readonly IRabbitMqHostTopology _topology;
        ConnectionScope _scope;
        readonly string _description;

        public RabbitMqConnectionCache(RabbitMqHostSettings settings, IRabbitMqHostTopology topology, ITaskSupervisor supervisor)
        {
            _settings = settings;
            _topology = topology;
            _connectionFactory = new Lazy<ConnectionFactory>(settings.GetConnectionFactory);

            _description = settings.ToDebugString();

            _cacheTaskScope = supervisor.CreateScope($"{TypeMetadataCache<RabbitMqConnectionCache>.ShortName} - {_description}", CloseScope);
        }

        public Task Send(IPipe<ConnectionContext> connectionPipe, CancellationToken cancellationToken)
        {
            ConnectionScope newScope = null;
            ConnectionScope existingScope;

            lock (_scopeLock)
            {
                existingScope = _scope;
                if (existingScope == null || existingScope.IsShuttingDown)
                {
                    newScope = new ConnectionScope(_cacheTaskScope, _description);
                    _scope = newScope;
                }
            }
            if (existingScope != null)
                return SendUsingExistingConnection(connectionPipe, existingScope, cancellationToken);

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

        Task CloseScope()
        {
            return TaskUtil.Completed;
        }

        Task SendUsingNewConnection(IPipe<ConnectionContext> connectionPipe, ConnectionScope scope, CancellationToken cancellationToken)
        {
            IConnection connection = null;
            try
            {
                if (_cacheTaskScope.StoppingToken.IsCancellationRequested)
                    throw new TaskCanceledException($"The connection is being disconnected: {_description}");

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connecting: {0}", _description);

                if (_settings.ClusterMembers?.Any() ?? false)
                {
                    connection = _connectionFactory.Value.CreateConnection(_settings.ClusterMembers, _settings.ClientProvidedName);
                }
                else
                {
                    List<string> hostNames = Enumerable.Repeat(_settings.Host, 1).ToList();

                    connection = _connectionFactory.Value.CreateConnection(hostNames, _settings.ClientProvidedName);
                }

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Connected: {0} (address: {1}, local: {2}", _description, connection.Endpoint, connection.LocalPort);
                }

                EventHandler<ShutdownEventArgs> connectionShutdown = null;
                connectionShutdown = (obj, reason) =>
                {
                    Interlocked.CompareExchange(ref _scope, null, scope);

                    scope.Shutdown(reason.ReplyText);

                    connection.ConnectionShutdown -= connectionShutdown;
                };

                connection.ConnectionShutdown += connectionShutdown;

                var connectionContext = new RabbitMqConnectionContext(connection, _settings, _topology, _description, _cacheTaskScope);

                connectionContext.GetOrAddPayload(() => _settings);

                scope.Connected(connectionContext);
            }
            catch (BrokerUnreachableException ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The broker was unreachable", ex);

                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.ConnectFaulted(ex);

                connection?.Dispose();

                throw new RabbitMqConnectionException("Connect failed: " + _description, ex);
            }
            catch (OperationInterruptedException ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The RabbitMQ operation was interrupted", ex);

                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.ConnectFaulted(ex);

                connection?.Dispose();

                throw new RabbitMqConnectionException("Operation interrupted: " + _description, ex);
            }

            return SendUsingExistingConnection(connectionPipe, scope, cancellationToken);
        }

        async Task SendUsingExistingConnection(IPipe<ConnectionContext> connectionPipe, ConnectionScope scope, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = await scope.Attach(cancellationToken).ConfigureAwait(false))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Using connection: {0}", _description);

                    await connectionPipe.Send(context).ConfigureAwait(false);
                }
            }
            catch (BrokerUnreachableException ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The broker was unreachable", ex);

                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.ConnectFaulted(ex);

                throw new RabbitMqConnectionException("Connect failed: " + _description, ex);
            }
            catch (OperationInterruptedException ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The RabbitMQ operation was interrupted", ex);

                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.ConnectFaulted(ex);

                throw new RabbitMqConnectionException("Operation interrupted: " + _description, ex);
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The connection usage threw an exception", ex);

                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.ConnectFaulted(ex);

                throw;
            }
        }


        class ConnectionScope
        {
            readonly TaskCompletionSource<RabbitMqConnectionContext> _connectionContext;
            readonly ITaskScope _taskScope;

            public ConnectionScope(ITaskScope scope, string debugString)
            {
                _connectionContext = new TaskCompletionSource<RabbitMqConnectionContext>();

                _taskScope = scope.CreateScope($"ConnectionScope: {debugString}", CloseContext);
            }

            public bool IsShuttingDown => _taskScope.StoppingToken.IsCancellationRequested;

            public void Connected(RabbitMqConnectionContext connectionContext)
            {
                _connectionContext.TrySetResult(connectionContext);

                _taskScope.SetReady();
            }

            public void ConnectFaulted(Exception exception)
            {
                _connectionContext.TrySetException(exception);

                _taskScope.SetNotReady(exception);

                _taskScope.Stop(new StopEventArgs($"Connection faulted: {exception.Message}"));
            }

            public async Task<SharedConnectionContext> Attach(CancellationToken cancellationToken)
            {
                var connectionContext = await _connectionContext.Task.ConfigureAwait(false);

                return new SharedConnectionContext(connectionContext, cancellationToken, _taskScope);
            }

            public void Shutdown(string reason)
            {
                _taskScope.Stop(new StopEventArgs(reason));
            }

            async Task CloseContext()
            {
                if (_connectionContext.Task.Status == TaskStatus.RanToCompletion)
                {
                    try
                    {
                        var connectionContext = await _connectionContext.Task.ConfigureAwait(false);

                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Disposing connection: {0}", connectionContext.HostSettings.ToDebugString());

                        connectionContext.Dispose();
                    }
                    catch (Exception exception)
                    {
                        if (_log.IsWarnEnabled)
                            _log.Warn("The model failed to be disposed", exception);
                    }
                }
            }
        }
    }
}