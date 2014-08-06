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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Policies;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;


    // this is a source vane, need to make it work like one - splice in the connection
    public interface IRabbitMqConnectionCache
    {
        Task Connect<T>(T context, IPipe<TupleContext<ConnectionContext, T>> next)
            where T : class, PipeContext;
    }


    public class RabbitMqConnectionCache : 
        IRabbitMqConnectionCache,
        IFilter<ConnectionContext>
    {
        readonly IRabbitMqConnector _connector;
        readonly object _lock = new object();

        ConnectionScope _scope;

        IPipe<ConnectionContext> _connectionPipe;

        public RabbitMqConnectionCache(IRabbitMqConnector connector)
        {
            _connector = connector;
            _connectionPipe = Pipe.New<ConnectionContext>(x =>
            {
                x.Filter(new DelegateFilter<ConnectionContext>(context =>
                {
                }));
            });
        }

        void RemoveConnectionContext(IConnection connection, ShutdownEventArgs reason)
        {
            lock (_lock)
            {
                _scope.ConnectionContext.Connection.ConnectionShutdown -= RemoveConnectionContext;
                _scope.ConnectionClosed.TrySetResult(true);
                _scope = default(ConnectionScope);
            }
        }

        public async Task Connect<T>(T context, IPipe<TupleContext<ConnectionContext, T>> next)
            where T : class, PipeContext
        {
            try
            {
                ConnectionScope connectionScope;
                lock (_lock)
                {
                    connectionScope = _scope;
                }

                if (connectionScope.ConnectionClosed.Task.Wait(TimeSpan.Zero))
                {
                    await next.Send(context.PushLeft(connectionScope.ConnectionContext));
                    return;
                }

                await _connector.Connect(_connectionPipe, context.CancellationToken);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public async Task Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            lock (_lock)
            {
                _scope = new ConnectionScope(context);
                context.Connection.ConnectionShutdown += RemoveConnectionContext;
            }
        }

        public bool Inspect(IPipeInspector inspector)
        {
            throw new NotImplementedException();
        }


        struct ConnectionScope
        {
            public readonly ConnectionContext ConnectionContext;
            public readonly TaskCompletionSource<bool> ConnectionClosed;

            public ConnectionScope(ConnectionContext connectionContext)
            {
                ConnectionContext = connectionContext;
                ConnectionClosed = new TaskCompletionSource<bool>();
            }
        }
    }



    /// <summary>
    /// Establishes connections to RabbitMQ using the specified retry policy
    /// </summary>
    public class RabbitMqConnector :
        IRabbitMqConnector
    {
        readonly ConnectionFactory _connectionFactory;
        readonly ILog _log = Logger.Get<RabbitMqConnector>();
        readonly IRetryPolicy _retryPolicy;

        public RabbitMqConnector(ConnectionFactory connectionFactory, IRetryPolicy retryPolicy)
        {
            _connectionFactory = connectionFactory;
            _retryPolicy = retryPolicy;
        }

        public Task Connect(IPipe<ConnectionContext> pipe, CancellationToken cancellationToken)
        {
            return _retryPolicy.Retry(async () =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connecting: {0}", _connectionFactory.ToDebugString());

                try
                {
                    using (IConnection connection = _connectionFactory.CreateConnection())
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Connected: {0}", _connectionFactory.ToDebugString());

                        using (var connectionContext = new RabbitMqConnectionContext(connection, _connectionFactory, cancellationToken))
                        {
                            await pipe.Send(connectionContext);
                        }

                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Disconnecting: {0}", _connectionFactory.ToDebugString());
                    }

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Disconnected: {0}", _connectionFactory.ToDebugString());
                }
                catch (BrokerUnreachableException ex)
                {
                    throw new RabbitMqConnectionException("Connect failed: " + _connectionFactory.ToDebugString(), ex);
                }
            }, cancellationToken);
        }
    }
}