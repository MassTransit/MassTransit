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
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using RabbitMQ.Client;


    public class RabbitMqConnectionCache :
        IRabbitMqConnector,
        IFilter<ConnectionContext>
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionCache>();
        readonly IRabbitMqConnector _connector;
        readonly object _lock = new object();
        readonly IPipe<ConnectionContext> _connectionPipe;
        ConnectionScope _scope;

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
            return inspector.Inspect(this);
        }

        public async Task Connect(IPipe<ConnectionContext> pipe, CancellationToken cancellationToken)
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
                    await pipe.Send(connectionScope.ConnectionContext);
                }

                await _connector.Connect(_connectionPipe, cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
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


        struct ConnectionScope
        {
            public readonly TaskCompletionSource<bool> ConnectionClosed;
            public readonly ConnectionContext ConnectionContext;

            public ConnectionScope(ConnectionContext connectionContext)
            {
                ConnectionContext = connectionContext;
                ConnectionClosed = new TaskCompletionSource<bool>();
            }
        }


    }
}