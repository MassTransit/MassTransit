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
    using Amazon.SQS.Model;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Pipeline;
    using Topology;


    public class SharedClientContext :
        ClientContext
    {
        readonly CancellationToken _cancellationToken;
        readonly ClientContext _context;
        readonly IPayloadCache _payloadCache;

        public SharedClientContext(ClientContext context, CancellationToken cancellationToken)
        {
            _context = context;
            _cancellationToken = cancellationToken;
        }

        public SharedClientContext(ClientContext context, IPayloadCache payloadCache, CancellationToken cancellationToken)
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

        ConnectionContext ClientContext.ConnectionContext => _context.ConnectionContext;

        IAmazonSqsPublishTopology ClientContext.PublishTopology => _context.PublishTopology;

        Task<string> ClientContext.CreateTopic(string topicName)
        {
            return _context.CreateTopic(topicName);
        }

        Task<string> ClientContext.CreateQueue(string queueName)
        {
            return _context.CreateQueue(queueName);
        }

        Task ClientContext.CreateQueueSubscription(string topicName, string queueName)
        {
            return _context.CreateQueueSubscription(topicName, queueName);
        }

        public Task CreateTopicSubscription(string sourceName, string destinationName)
        {
            return _context.CreateTopicSubscription(sourceName, destinationName);
        }

        Task ClientContext.DeleteTopic(string topicName)
        {
            return _context.DeleteTopic(topicName);
        }

        Task ClientContext.DeleteQueue(string queueName)
        {
            return _context.DeleteQueue(queueName);
        }

        Task ClientContext.BasicConsume(ReceiveSettings receiveSettings, IBasicConsumer consumer)
        {
            return _context.BasicConsume(receiveSettings, consumer);
        }

        PublishRequest ClientContext.CreatePublishRequest(string topicName, byte[] body)
        {
            return _context.CreatePublishRequest(topicName, body);
        }

        Task ClientContext.Publish(PublishRequest request, CancellationToken cancellationToken)
        {
            return _context.Publish(request, cancellationToken);
        }

        Task ClientContext.DeleteMessage(string queueUrl, string receiptHandle, CancellationToken cancellationToken)
        {
            return _context.DeleteMessage(queueUrl, receiptHandle, cancellationToken);
        }

        Task ClientContext.PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            return _context.PurgeQueue(queueName, cancellationToken);
        }

        SendMessageRequest ClientContext.CreateSendRequest(string queueName, byte[] body)
        {
            return _context.CreateSendRequest(queueName, body);
        }

        Task ClientContext.SendMessage(SendMessageRequest request, CancellationToken cancellationToken)
        {
            return _context.SendMessage(request, cancellationToken);
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;
    }
}
