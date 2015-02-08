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
namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Threading;
    using Context;
    using RabbitMQ.Client;


    public class RabbitMqModelContext :
        ModelContext,
        IDisposable
    {
        readonly ConnectionContext _connectionContext;
        readonly IModel _model;
        readonly IHaModel _haModel;
        readonly PayloadCache _payloadCache;
        readonly CancellationTokenSource _tokenSource;
        CancellationTokenRegistration _registration;

        public RabbitMqModelContext(ConnectionContext connectionContext, IModel model, CancellationToken cancellationToken)
        {
            _model = model;
            _haModel = new HaModel(connectionContext, model);
            _connectionContext = connectionContext;
            _payloadCache = new PayloadCache();

            _tokenSource = new CancellationTokenSource();
            _registration = cancellationToken.Register(OnCancellationRequested);

            model.ModelShutdown += OnModelShutdown;
        }

        public void Dispose()
        {
            _model.ModelShutdown -= OnModelShutdown;

            _registration.Dispose();

            _model.Cleanup();
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


        public IHaModel Model
        {
            get { return _haModel; }
        }

        public ConnectionContext ConnectionContext
        {
            get { return _connectionContext; }
        }

        public CancellationToken CancellationToken
        {
            get { return _tokenSource.Token; }
        }

        void OnModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            _tokenSource.Cancel();

            _model.Cleanup();
        }

        void OnCancellationRequested()
        {
            _tokenSource.Cancel();
        }
    }
}