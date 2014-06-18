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
namespace MassTransit.Transports.RabbitMq.Contexts
{
    using System;
    using System.Threading;
    using Context;
    using RabbitMQ.Client;


    public class RabbitMqConnectionContext :
        ConnectionContext,
        IDisposable
    {
        readonly object _lock = new object();
        readonly PayloadCache _payloadCache;
        readonly CancellationTokenSource _tokenSource;
        IConnection _connection;
        CancellationTokenRegistration _registration;

        public RabbitMqConnectionContext(IConnection connection, CancellationToken cancellationToken)
        {
            _connection = connection;
            _payloadCache = new PayloadCache();

            _tokenSource = new CancellationTokenSource();
            _registration = cancellationToken.Register(OnCancellationRequested);

            connection.ConnectionShutdown += OnConnectionShutdown;
        }

        public bool HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            return _payloadCache.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            return _payloadCache.GetOrAddPayload(payloadFactory);
        }


        public IConnection Connection
        {
            get
            {
                lock (_lock)
                {
                    if (_connection == null)
                        throw new InvalidOperationException("The connection was closed");

                    return _connection;
                }
            }
        }

        public CancellationToken CancellationToken
        {
            get { return _tokenSource.Token; }
        }

        public void Dispose()
        {
            _connection.ConnectionShutdown -= OnConnectionShutdown;

            Close();
        }

        void OnConnectionShutdown(IConnection connection, ShutdownEventArgs reason)
        {
            _tokenSource.Cancel();

            Close();
        }

        void OnCancellationRequested()
        {
            _tokenSource.Cancel();
        }

        void Close()
        {
            lock (_lock)
            {
                _registration.Dispose();

                _connection.Cleanup();
                _connection = null;
            }
        }
    }
}