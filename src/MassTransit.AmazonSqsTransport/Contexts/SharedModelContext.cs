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
namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService.Model;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Pipeline;
    using Topology;


    public class SharedModelContext :
        ModelContext
    {
        readonly CancellationToken _cancellationToken;
        readonly ModelContext _context;
        readonly IPayloadCache _payloadCache;

        public SharedModelContext(ModelContext context, CancellationToken cancellationToken)
        {
            _context = context;
            _cancellationToken = cancellationToken;
        }

        public SharedModelContext(ModelContext context, IPayloadCache payloadCache, CancellationToken cancellationToken)
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

        ConnectionContext ModelContext.ConnectionContext => _context.ConnectionContext;

        IAmazonSqsPublishTopology ModelContext.PublishTopology => _context.PublishTopology;

        Task<string> ModelContext.GetTopic(string topicName)
        {
            return _context.GetTopic(topicName);
        }

        Task<string> ModelContext.GetQueue(string queueName)
        {
            return _context.GetQueue(queueName);
        }

        Task ModelContext.GetTopicSubscription(string topicName, string queueName)
        {
            return _context.GetTopicSubscription(topicName, queueName);
        }

        Task ModelContext.DeleteTopic(string topicName)
        {
            return _context.DeleteTopic(topicName);
        }

        Task ModelContext.DeleteQueue(string queueName)
        {
            return _context.DeleteQueue(queueName);
        }

        Task ModelContext.BasicConsume(string queueUrl, int prefetchCount, AmazonSqsBasicConsumer consumer)
        {
            return _context.BasicConsume(queueUrl, prefetchCount, consumer);
        }

        PublishRequest ModelContext.CreateTransportMessage(string topicArn, byte[] body)
        {
            return _context.CreateTransportMessage(topicArn, body);
        }

        Task ModelContext.Publish(PublishRequest publishRequest, CancellationToken cancellationToken)
        {
            return _context.Publish(publishRequest, cancellationToken);
        }

        Task ModelContext.DeleteMessage(string queueUrl, string receiptHandle, CancellationToken cancellationToken)
        {
            return _context.DeleteMessage(queueUrl, receiptHandle, cancellationToken);
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;
    }
}
