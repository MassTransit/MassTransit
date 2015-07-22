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
namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;


    public class RabbitMqConnectionCache :
        IConnectionCache,
        IProbeSite
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionCache>();
        readonly ConnectionFactory _connectionFactory;
        readonly object _scopeLock = new object();
        readonly RabbitMqHostSettings _settings;
        ConnectionScope _scope;

        public RabbitMqConnectionCache(RabbitMqHostSettings settings)
        {
            _settings = settings;

            _connectionFactory = settings.GetConnectionFactory();
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
                    newScope = new ConnectionScope();
                    _scope = newScope;
                }
            }
            if (existingScope != null)
                return SendUsingExistingConnection(connectionPipe, cancellationToken, existingScope);

            return SendUsingNewConnection(connectionPipe, newScope, cancellationToken);
        }

        public void Probe(ProbeContext context)
        {
            ConnectionScope connectionScope = _scope;
            if (connectionScope != null)
            {
                context.Set(new
                {
                    Connected = true
                });
            }
        }

        async Task SendUsingNewConnection(IPipe<ConnectionContext> connectionPipe, ConnectionScope scope, CancellationToken cancellationToken)
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connecting: {0}", _connectionFactory.ToDebugString());

                IConnection connection = _connectionFactory.CreateConnection();

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Connected: {0} (address: {1}, local: {2}", _connectionFactory.ToDebugString(),
                        connection.RemoteEndPoint, connection.LocalEndPoint);
                }

                EventHandler<ShutdownEventArgs> connectionShutdown = null;
                connectionShutdown = (obj, reason) =>
                {
                    connection.ConnectionShutdown -= connectionShutdown;

                    Interlocked.CompareExchange(ref _scope, null, scope);

                    scope.Close();
                };

                connection.ConnectionShutdown += connectionShutdown;

                var connectionContext = new RabbitMqConnectionContext(connection, _settings, cancellationToken);

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
                using (SharedConnectionContext context = await scope.Attach(cancellationToken))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Using new connection: {0}", ((ConnectionContext)context).HostSettings.ToDebugString());

                    await connectionPipe.Send(context);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The connection usage threw an exception", ex);

                throw;
            }
        }

        static async Task SendUsingExistingConnection(IPipe<ConnectionContext> connectionPipe, CancellationToken cancellationToken, ConnectionScope scope)
        {
            try
            {
                using (SharedConnectionContext context = await scope.Attach(cancellationToken))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Using existing connection: {0}", ((ConnectionContext)context).HostSettings.ToDebugString());

                    await connectionPipe.Send(context);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The existing connection usage threw an exception", ex);

                throw;
            }
        }


        class ConnectionScope
        {
            static readonly ILog _log = Logger.Get<ConnectionScope>();
            readonly ConcurrentBag<SharedConnectionContext> _attached;
            readonly TaskCompletionSource<RabbitMqConnectionContext> _connectionContext;

            public ConnectionScope()
            {
                _attached = new ConcurrentBag<SharedConnectionContext>();
                _connectionContext = new TaskCompletionSource<RabbitMqConnectionContext>();
            }

            public void Connected(RabbitMqConnectionContext connectionContext)
            {
                _connectionContext.TrySetResult(connectionContext);
            }

            public async Task<SharedConnectionContext> Attach(CancellationToken cancellationToken)
            {
                var context = new SharedConnectionContext(await _connectionContext.Task, cancellationToken);

                _attached.Add(context);
                return context;
            }

            public async void Close()
            {
                var connectionContext = await _connectionContext.Task;

                try
                {
                    foreach (var context in _attached)
                    {
                        using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                        {
                            try
                            {
                                await context.Completed.WithCancellation(source.Token);
                            }
                            catch (OperationCanceledException)
                            {
                            }
                        }
                    }

                    await Task.WhenAll(_attached.Select(x => x.Completed));
                }
                catch (Exception ex)
                {
                    _log.Error($"Close Faulted waiting for shared contexts", ex);
                }

                connectionContext.Dispose();

                await connectionContext.Completed;
            }

            public void ConnectFaulted(Exception exception)
            {
                _connectionContext.TrySetException(exception);
            }
        }
    }
}