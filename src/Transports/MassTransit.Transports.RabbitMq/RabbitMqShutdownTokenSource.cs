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
    using RabbitMQ.Client;


    public class RabbitMqShutdownTokenSource :
        IDisposable
    {
        readonly Action _onDispose;
        readonly CancellationTokenSource _tokenSource;
        CancellationTokenRegistration _registration;

        public RabbitMqShutdownTokenSource(IConnection connection, CancellationToken cancellationToken)
        {
            _tokenSource = new CancellationTokenSource();
            _registration = cancellationToken.Register(OnCancellationRequested);

            connection.ConnectionShutdown += OnConnectionShutdown;
            _onDispose = () => connection.ConnectionShutdown -= OnConnectionShutdown;
        }

        public RabbitMqShutdownTokenSource(IModel model, CancellationToken cancellationToken)
        {
            _tokenSource = new CancellationTokenSource();
            _registration = cancellationToken.Register(OnCancellationRequested);

            model.ModelShutdown += OnModelShutdown;
            _onDispose = () => model.ModelShutdown -= OnModelShutdown;
        }

        public CancellationToken Token
        {
            get { return _tokenSource.Token; }
        }

        public void Dispose()
        {
            _registration.Dispose();
            _onDispose();
        }

        void OnCancellationRequested()
        {
            _tokenSource.Cancel();
        }

        void OnModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            _tokenSource.Cancel();
        }

        void OnConnectionShutdown(IConnection connection, ShutdownEventArgs reason)
        {
            _tokenSource.Cancel();
        }
    }
}