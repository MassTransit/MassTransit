namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using MassTransit.Middleware;
    using Topology;
    using Transports;


    public class AmazonSqsClientContext :
        ScopePipeContext,
        ClientContext
    {
        readonly IAmazonSimpleNotificationService _snsClient;
        readonly IAmazonSQS _sqsClient;

        public AmazonSqsClientContext(ConnectionContext connectionContext,
            IAmazonSQS sqsClient,
            IAmazonSimpleNotificationService snsClient,
            CancellationToken cancellationToken)
            : base(connectionContext)
        {
            ConnectionContext = connectionContext;

            _sqsClient = sqsClient;
            _snsClient = snsClient;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public ConnectionContext ConnectionContext { get; }

        public Task<TopicInfo> CreateTopic(Topology.Topic topic)
        {
            return ConnectionContext.GetTopic(topic);
        }

        public Task<QueueInfo> CreateQueue(Queue queue)
        {
            return ConnectionContext.GetQueue(queue);
        }

        public async Task<bool> CreateQueueSubscription(Topology.Topic topic, Queue queue)
        {
            var topicInfo = await ConnectionContext.GetTopic(topic).ConfigureAwait(false);
            var queueInfo = await ConnectionContext.GetQueue(queue).ConfigureAwait(false);

            Dictionary<string, string> subscriptionAttributes = topic.TopicSubscriptionAttributes.Select(x => (x.Key, x.Value.ToString()))
                .Concat(queue.QueueSubscriptionAttributes.Select(x => (x.Key, x.Value.ToString())))
                .ToDictionary(x => x.Item1, x => x.Item2);

            var subscribeRequest = new SubscribeRequest
            {
                TopicArn = topicInfo.Arn,
                Endpoint = queueInfo.Arn,
                Protocol = "sqs",
                Attributes = subscriptionAttributes
            };

            string subscriptionArn = null;
            try
            {
                var response = await _snsClient.SubscribeAsync(subscribeRequest, CancellationToken).ConfigureAwait(false);

                response.EnsureSuccessfulResponse();

                subscriptionArn = response.SubscriptionArn;
            }
            catch (InvalidParameterException exception) when (exception.Message.Contains("exists"))
            {
                try
                {
                    var existingSubscriptions = await _snsClient.ListSubscriptionsByTopicAsync(topicInfo.Arn, CancellationToken).ConfigureAwait(false);
                    existingSubscriptions.EnsureSuccessfulResponse();

                    var existingSubscription = existingSubscriptions.Subscriptions.SingleOrDefault(x =>
                        x.TopicArn == topicInfo.Arn && x.Endpoint == queueInfo.Arn && x.Protocol == "sqs");

                    if (existingSubscription != null)
                    {
                        subscriptionArn = existingSubscription.SubscriptionArn;
                        var attributes = await _snsClient.GetSubscriptionAttributesAsync(subscriptionArn, CancellationToken)
                            .ConfigureAwait(false);

                        if (attributes.HttpStatusCode is >= HttpStatusCode.OK and < HttpStatusCode.MultipleChoices)
                        {
                            foreach (var (name, value) in SubscriptionAttributesEqual(attributes.Attributes, subscriptionAttributes))
                            {
                                var request = new SetSubscriptionAttributesRequest
                                {
                                    AttributeName = name,
                                    AttributeValue = value,
                                    SubscriptionArn = subscriptionArn
                                };

                                var updated = await _snsClient.SetSubscriptionAttributesAsync(request, CancellationToken).ConfigureAwait(false);
                                updated.EnsureSuccessfulResponse();

                                LogContext.Debug?.Log("Updated subscription attribute: {SubscriptionArn} {Name}={Value}", subscriptionArn, name,
                                    value);
                            }
                        }
                    }
                }
                catch (Exception updateException)
                {
                    LogContext.Warning?.Log(updateException, "Failed to update subscription attributes: {SubscriptionArg}", subscriptionArn);
                }

                if (subscriptionArn == null)
                    return false;
            }

            queueInfo.SubscriptionArns.Add(subscriptionArn);

            var sqsQueueArn = queueInfo.Arn;

            return await queueInfo.UpdatePolicy(sqsQueueArn, topicInfo.Arn, CancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteTopic(Topology.Topic topic)
        {
            var topicInfo = await ConnectionContext.GetTopic(topic).ConfigureAwait(false);

            TransportLogMessages.DeleteTopic(topicInfo.Arn);

            var response = await _snsClient.DeleteTopicAsync(topicInfo.Arn, CancellationToken.None).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            await ConnectionContext.RemoveTopicByName(topic.EntityName).ConfigureAwait(false);
        }

        public async Task DeleteQueue(Queue queue)
        {
            var queueInfo = await ConnectionContext.GetQueue(queue).ConfigureAwait(false);

            TransportLogMessages.DeleteQueue(queueInfo.Url);

            foreach (var subscriptionArn in queueInfo.SubscriptionArns)
            {
                TransportLogMessages.DeleteSubscription(queueInfo.Url, subscriptionArn);

                await DeleteQueueSubscription(subscriptionArn).ConfigureAwait(false);
            }

            var response = await _sqsClient.DeleteQueueAsync(queueInfo.Url, CancellationToken.None).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            await ConnectionContext.RemoveQueueByName(queue.EntityName).ConfigureAwait(false);
        }

        public async Task Publish(string topicName, PublishBatchRequestEntry request, CancellationToken cancellationToken)
        {
            var topicInfo = await ConnectionContext.GetTopicByName(topicName).ConfigureAwait(false);

            await topicInfo.Publish(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task SendMessage(string queueName, SendMessageBatchRequestEntry request, CancellationToken cancellationToken)
        {
            var queueInfo = await ConnectionContext.GetQueueByName(queueName).ConfigureAwait(false);

            await queueInfo.Send(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteMessage(string queueName, string receiptHandle, CancellationToken cancellationToken)
        {
            var queueInfo = await ConnectionContext.GetQueueByName(queueName).ConfigureAwait(false);

            await queueInfo.Delete(receiptHandle, cancellationToken).ConfigureAwait(false);
        }

        public async Task PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            var queueInfo = await ConnectionContext.GetQueueByName(queueName).ConfigureAwait(false);

            var response = await _sqsClient.PurgeQueueAsync(queueInfo.Url, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();
        }

        public async Task<IList<Message>> ReceiveMessages(string queueName, int messageLimit, int waitTime, CancellationToken cancellationToken)
        {
            var queueInfo = await ConnectionContext.GetQueueByName(queueName).ConfigureAwait(false);

            var request = new ReceiveMessageRequest(queueInfo.Url)
            {
                MaxNumberOfMessages = messageLimit,
                WaitTimeSeconds = waitTime,
                #pragma warning disable CS0618 // Type or member is obsolete
                AttributeNames = ["All"],
                #pragma warning restore CS0618 // Type or member is obsolete
                MessageAttributeNames = ["All"]
            };

            var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            return response.Messages;
        }

        public Task<QueueInfo> GetQueueInfo(string queueName)
        {
            return ConnectionContext.GetQueueByName(queueName);
        }

        public async Task ChangeMessageVisibility(string queueUrl, string receiptHandle, int seconds)
        {
            var response = await _sqsClient.ChangeMessageVisibilityAsync(new ChangeMessageVisibilityRequest
            {
                QueueUrl = queueUrl,
                ReceiptHandle = receiptHandle,
                VisibilityTimeout = seconds
            }, CancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();
        }

        async Task DeleteQueueSubscription(string subscriptionArn)
        {
            var unsubscribeRequest = new UnsubscribeRequest { SubscriptionArn = subscriptionArn };

            var response = await _snsClient.UnsubscribeAsync(unsubscribeRequest, CancellationToken.None).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();
        }

        static IEnumerable<(string, string)> SubscriptionAttributesEqual(Dictionary<string, string> existingAttributes,
            Dictionary<string, string> updatedAttributes)
        {
            if (updatedAttributes.TryGetValue("FilterPolicy", out var filterPolicy))
            {
                if (!existingAttributes.TryGetValue("FilterPolicy", out var existingFilterPolicy) || existingFilterPolicy != filterPolicy)
                    yield return ("FilterPolicy", filterPolicy);
            }

            if (updatedAttributes.TryGetValue("FilterPolicyScope", out var filterPolicyScope))
            {
                if (!existingAttributes.TryGetValue("FilterPolicyScope", out var existingFilterPolicyScope) || existingFilterPolicyScope != filterPolicyScope)
                    yield return ("FilterPolicyScope", filterPolicyScope);
            }

            if (updatedAttributes.TryGetValue("RawMessageDelivery", out var rawMessageDelivery))
            {
                if (!existingAttributes.TryGetValue("RawMessageDelivery", out var existingRawMessageDelivery)
                    || existingRawMessageDelivery != rawMessageDelivery)
                    yield return ("RawMessageDelivery", rawMessageDelivery);
            }
        }
    }
}
