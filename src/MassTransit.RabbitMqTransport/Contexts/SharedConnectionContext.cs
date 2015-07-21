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
namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using RabbitMQ.Client;


    public class SharedConnectionContext :
        ConnectionContext,
        IDisposable
    {
        readonly CancellationToken _cancellationToken;
        readonly TaskCompletionSource<bool> _completed;
        readonly ConnectionContext _context;

        public SharedConnectionContext(ConnectionContext context, CancellationToken cancellationToken)
        {
            _context = context;
            _cancellationToken = cancellationToken;
            _completed = new TaskCompletionSource<bool>();
        }

        public Task Completed => _completed.Task;
        CancellationToken PipeContext.CancellationToken => _cancellationToken;

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

        IConnection ConnectionContext.Connection => _context.Connection;
        RabbitMqHostSettings ConnectionContext.HostSettings => _context.HostSettings;

        Task<IModel> ConnectionContext.CreateModel()
        {
            return _context.CreateModel();
        }

        void IDisposable.Dispose()
        {
            _completed.TrySetResult(true);
        }
    }
}