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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Lambda;
    using Amazon.Lambda.Model;
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Logging;
    using Pipeline;
    using Topology;
    using Util;


    public class AmazonSqsClientContext :
        BasePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        static readonly ILog _log = Logger.Get<AmazonSqsClientContext>();

        readonly ConnectionContext _connectionContext;
        readonly IAmazonSqsHost _host;
        readonly IAmazonSQS _amazonSqs;
        readonly IAmazonSimpleNotificationService _amazonSns;
        readonly IAmazonLambda _amazonLambda;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;
        readonly object _lock = new object();
        readonly IDictionary<string, string> _queueUrls;
        readonly IDictionary<string, string> _topicArns;

        public AmazonSqsClientContext(
            ConnectionContext connectionContext,
            IAmazonSQS amazonSqs,
            IAmazonSimpleNotificationService amazonSns,
            IAmazonLambda amazonLambda,
            IAmazonSqsHost host,
            CancellationToken cancellationToken)
            : base(new PayloadCacheScope(connectionContext), cancellationToken)
        {
            _connectionContext = connectionContext;
            _amazonSqs = amazonSqs;
            _amazonSns = amazonSns;
            _amazonLambda = amazonLambda;
            _host = host;

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);

            _queueUrls = new Dictionary<string, string>();
            _topicArns = new Dictionary<string, string>();
        }

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Closing model: {0}", _connectionContext.Description);

            _amazonSqs?.Dispose();
            _amazonSns?.Dispose();
            _amazonLambda?.Dispose();

            return GreenPipes.Util.TaskUtil.Completed;
        }

        IAmazonSqsPublishTopology ClientContext.PublishTopology => _host.Topology.PublishTopology;

        ConnectionContext ClientContext.ConnectionContext => _connectionContext;

        public async Task<string> CreateTopic(string topicName)
        {
            lock (_lock)
                if (_topicArns.TryGetValue(topicName, out var result))
                    return result;

            var response = await _amazonSns.CreateTopicAsync(topicName).ConfigureAwait(false);

            var topicArn = response.TopicArn;

            lock (_lock)
                _topicArns[topicName] = topicArn;

            return topicArn;
        }

        public async Task<string> CreateQueue(string queueName)
        {
            lock (_lock)
                if (_queueUrls.TryGetValue(queueName, out var result))
                    return result;

            var response = await _amazonSqs.CreateQueueAsync(queueName).ConfigureAwait(false);

            var queueUrl = response.QueueUrl;

            lock (_lock)
                _queueUrls[queueName] = queueUrl;

            return queueUrl;
        }

        async Task ClientContext.CreateQueueSubscription(string topicName, string queueName)
        {
            var results = await Task.WhenAll(CreateTopic(topicName), CreateQueue(queueName)).ConfigureAwait(false);

            var topicArn = results[0];
            var queueUrl = results[1];

            var subscriptionArn = await _amazonSns.SubscribeQueueAsync(topicArn, _amazonSqs, queueUrl).ConfigureAwait(false);

            await _amazonSns.SetSubscriptionAttributesAsync(subscriptionArn, "RawMessageDelivery", "true").ConfigureAwait(false);
        }

        async Task ClientContext.CreateTopicSubscription(string sourceName, string destinationName)
        {
            var results = await Task.WhenAll(CreateTopic(sourceName), CreateTopic(destinationName)).ConfigureAwait(false);

            var sourceArn = results[0];
            var destinationArn = results[1];

            var functionName = $"MT_{sourceName}";

            if(await FunctionExits(functionName).ConfigureAwait(false)) return;

            using (var resourceStream = GetType().Assembly.GetManifestResourceStream("MassTransit.AmazonSqsTransport.TopicSubscription.zip"))
            using (var zipFile = new MemoryStream())
            {
                await resourceStream.CopyToAsync(zipFile);

                var response = await _amazonLambda.CreateFunctionAsync(new CreateFunctionRequest
                {
                    Code = new FunctionCode {ZipFile = zipFile},
                    Runtime = Runtime.Dotnetcore21,
                    Handler = "MassTransit.AmazonSqsTransport.TopicSubscription::MassTransit.AmazonSqsTransport.TopicSubscription.TopicSubscription::Handler",
                    FunctionName = functionName,
                    Role = _host.Settings.ExecutionRoleArn,
                    Environment = new Amazon.Lambda.Model.Environment {Variables = {{"PUBLISH_TOPIC_ARN", destinationArn}}},
                    Timeout = 30,
                    MemorySize = 256
                }).ConfigureAwait(false);

                await _amazonLambda.AddPermissionAsync(new Amazon.Lambda.Model.AddPermissionRequest
                {
                    StatementId = "AllowExecutionFromSNS",
                    Action = "lambda:InvokeFunction",
                    FunctionName = functionName,
                    Principal = "sns.amazonaws.com",
                    SourceArn = sourceArn
                });

                await _amazonSns.SubscribeAsync(new SubscribeRequest
                {
                    TopicArn = sourceArn,
                    Endpoint = response.FunctionArn,
                    Protocol = "lambda"
                }).ConfigureAwait(false);
            }
        }

        async Task<bool> FunctionExits(string functionName)
        {
            try
            {
                await _amazonLambda.GetFunctionConfigurationAsync(functionName);
                return true;
            }
            catch(ResourceNotFoundException)
            {
                return false;
            }
        }

        async Task ClientContext.DeleteTopic(string topicName)
        {
            var topicArn = await CreateTopic(topicName).ConfigureAwait(false);
            await _amazonSns.DeleteTopicAsync(topicArn).ConfigureAwait(false);
        }

        async Task ClientContext.DeleteQueue(string queueName)
        {
            var queueUrl = await CreateQueue(queueName).ConfigureAwait(false);
            await _amazonSqs.DeleteQueueAsync(queueUrl).ConfigureAwait(false);
        }

        Task ClientContext.BasicConsume(ReceiveSettings receiveSettings, IBasicConsumer consumer)
        {
            string queueUrl;
            lock (_lock)
            {
                if (!_queueUrls.TryGetValue(receiveSettings.EntityName, out queueUrl))
                    throw new ArgumentException($"The queue was unknown: {receiveSettings.EntityName}", nameof(receiveSettings));
            }

            return Task.Factory.StartNew(async () =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    var request = new ReceiveMessageRequest(queueUrl)
                    {
                        MaxNumberOfMessages = receiveSettings.PrefetchCount,
                        WaitTimeSeconds = receiveSettings.WaitTimeSeconds,
                        AttributeNames = new List<string> {"All"},
                        MessageAttributeNames = new List<string> {"All"}
                    };

                    var response = await _amazonSqs.ReceiveMessageAsync(request, CancellationToken).ConfigureAwait(false);

                    await Task.WhenAll(response.Messages.Select(consumer.HandleMessage)).ConfigureAwait(false);
                }
            }, CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        PublishRequest ClientContext.CreatePublishRequest(string topicName, byte[] body)
        {
            var message = Encoding.UTF8.GetString(body);

            lock (_lock)
                if (_topicArns.TryGetValue(topicName, out var topicArn))
                    return new PublishRequest(topicArn, message);

            throw new ArgumentException($"The topic was unknown: {topicName}", nameof(topicName));
        }

        SendMessageRequest ClientContext.CreateSendRequest(string queueName, byte[] body)
        {
            var message = Encoding.UTF8.GetString(body);

            lock (_lock)
                if (_queueUrls.TryGetValue(queueName, out var queueUrl))
                    return new SendMessageRequest(queueUrl, message);

            throw new ArgumentException($"The queue was unknown: {queueName}", nameof(queueName));
        }

        Task ClientContext.Publish(PublishRequest request, CancellationToken cancellationToken)
        {
            return _amazonSns.PublishAsync(request, cancellationToken);
        }

        Task ClientContext.SendMessage(SendMessageRequest request, CancellationToken cancellationToken)
        {
            return _amazonSqs.SendMessageAsync(request, cancellationToken);
        }

        Task ClientContext.DeleteMessage(string queueName, string receiptHandle, CancellationToken cancellationToken)
        {
            lock (_lock)
                if (_queueUrls.TryGetValue(queueName, out var queueUrl))
                    return _amazonSqs.DeleteMessageAsync(queueUrl, receiptHandle, cancellationToken);

            throw new ArgumentException($"The queue was unknown: {queueName}", nameof(queueName));
        }

        Task ClientContext.PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            lock (_lock)
                if (_queueUrls.TryGetValue(queueName, out var queueUrl))
                    return _amazonSqs.PurgeQueueAsync(queueUrl, cancellationToken);

            throw new ArgumentException($"The queue was unknown: {queueName}", nameof(queueName));
        }
    }
}
