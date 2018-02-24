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
namespace MassTransit.HttpTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Hosting;
    using Transport;


    public class SharedHttpHttpHostContext :
        HttpHostContext
    {
        readonly HttpHostContext _context;

        public SharedHttpHttpHostContext(HttpHostContext context, CancellationToken cancellationToken)
        {
            _context = context;

            CancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken { get; }

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        public HttpHostSettings HostSettings => _context.HostSettings;

        void HttpHostContext.RegisterEndpointHandler(string pathMatch, HttpConsumer handler)
        {
            _context.RegisterEndpointHandler(pathMatch, handler);
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            return _context.Stop(cancellationToken);
        }

        public Task Start(CancellationToken cancellationToken)
        {
            return _context.Start(cancellationToken);
        }
    }
}