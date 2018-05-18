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
namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Topology;


    public class SharedSessionContext :
        SessionContext
    {
        readonly CancellationToken _cancellationToken;
        readonly SessionContext _context;
        readonly IPayloadCache _payloadCache;

        public SharedSessionContext(SessionContext context, CancellationToken cancellationToken)
        {
            _context = context;
            _cancellationToken = cancellationToken;
        }

        public SharedSessionContext(SessionContext context, IPayloadCache payloadCache, CancellationToken cancellationToken)
        {
            _context = context;
            _payloadCache = payloadCache;
            _cancellationToken = cancellationToken;
        }

        bool PipeContext.HasPayloadType(Type contextType)
        {
            if (_payloadCache != null)
                return _payloadCache.HasPayloadType(contextType);

            return _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            if (_payloadCache != null)
                return _payloadCache.TryGetPayload(out payload);

            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            if (_payloadCache != null)
                return _payloadCache.GetOrAddPayload(payloadFactory);

            return _context.GetOrAddPayload(payloadFactory);
        }

        T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        public ISession Session => _context.Session;

        ConnectionContext SessionContext.ConnectionContext => _context.ConnectionContext;

        IActiveMqPublishTopology SessionContext.PublishTopology => _context.PublishTopology;

        Task<ITopic> SessionContext.GetTopic(string topicName)
        {
            return _context.GetTopic(topicName);
        }

        Task<IQueue> SessionContext.GetQueue(string queueName)
        {
            return _context.GetQueue(queueName);
        }

        Task<IDestination> SessionContext.GetDestination(string destination, DestinationType destinationType)
        {
            return _context.GetDestination(destination, destinationType);
        }

        Task<IMessageProducer> SessionContext.CreateMessageProducer(IDestination destination)
        {
            return _context.CreateMessageProducer(destination);
        }

        Task<IMessageConsumer> SessionContext.CreateMessageConsumer(IDestination destination, string selector, bool noLocal)
        {
            return _context.CreateMessageConsumer(destination, selector, noLocal);
        }

        Task SessionContext.DeleteTopic(string topicName)
        {
            return _context.DeleteTopic(topicName);
        }

        Task SessionContext.DeleteQueue(string queueName)
        {
            return _context.DeleteQueue(queueName);
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;
    }
}