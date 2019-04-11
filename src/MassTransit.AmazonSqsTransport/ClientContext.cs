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
namespace MassTransit.AmazonSqsTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService.Model;
    using Amazon.SQS.Model;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Queue = Topology.Entities.Queue;
    using Topic = Topology.Entities.Topic;


    public interface ClientContext :
        PipeContext
    {
        /// <summary>
        /// The connection context for the model
        /// </summary>
        ConnectionContext ConnectionContext { get; }

        Task<string> CreateTopic(Topic topic);

        Task<string> CreateQueue(Queue queue);

        Task CreateQueueSubscription(Topic topic, Queue queue);

        Task DeleteTopic(Topic topic);

        Task DeleteQueue(Queue queue);

        Task BasicConsume(ReceiveSettings receiveSettings, IBasicConsumer consumer);

        PublishRequest CreatePublishRequest(string topicName, byte[] body);

        Task Publish(PublishRequest request, CancellationToken cancellationToken = default);

        SendMessageRequest CreateSendRequest(string queueName, byte[] body);

        Task SendMessage(SendMessageRequest request, CancellationToken cancellationToken);

        Task DeleteMessage(string queueUrl, string receiptHandle, CancellationToken cancellationToken = default);
        Task PurgeQueue(string queueName, CancellationToken cancellationToken);
    }
}
