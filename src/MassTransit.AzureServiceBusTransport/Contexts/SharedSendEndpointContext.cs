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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;


    public class SharedSendEndpointContext :
        SendEndpointContext
    {
        readonly SendEndpointContext _context;

        public SharedSendEndpointContext(SendEndpointContext context, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            _context = context;
        }

        bool PipeContext.HasPayloadType(Type payloadType)
        {
            return _context.HasPayloadType(payloadType);
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

        public CancellationToken CancellationToken { get; }

        string SendEndpointContext.EntityPath => _context.EntityPath;

        Task SendEndpointContext.Send(BrokeredMessage message)
        {
            return _context.Send(message);
        }

        Task<long> SendEndpointContext.ScheduleSend(BrokeredMessage message, DateTime scheduleEnqueueTimeUtc)
        {
            return _context.ScheduleSend(message, scheduleEnqueueTimeUtc);
        }

        Task SendEndpointContext.CancelScheduledSend(long sequenceNumber)
        {
            return _context.CancelScheduledSend(sequenceNumber);
        }
    }
}