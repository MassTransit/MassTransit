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


    public class RabbitMqModelContext :
        ModelContext,
        IDisposable
    {
        readonly object _lock = new object();
        readonly CancellationTokenSource _tokenSource;
        IModel _model;
        CancellationTokenRegistration _registration;

        public RabbitMqModelContext(IModel model, CancellationToken cancellationToken)
        {
            _model = model;

            _tokenSource = new CancellationTokenSource();
            _registration = cancellationToken.Register(OnCancellationRequested);

            model.ModelShutdown += OnModelShutdown;
        }

        public void Dispose()
        {
            _model.ModelShutdown -= OnModelShutdown;

            Close();
        }

        public IModel Model
        {
            get { return _model; }
        }

        public CancellationToken CancellationToken
        {
            get { return _tokenSource.Token; }
        }

        void OnModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            _tokenSource.Cancel();

            Close();
        }

        void Close()
        {
            lock (_lock)
            {
                _registration.Dispose();

                _model.Cleanup();
                _model = null;
            }
        }

        void OnCancellationRequested()
        {
            _tokenSource.Cancel();
        }
    }
}