namespace MassTransit.SqlTransport.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Data.SqlClient;
    using Serialization;
    using Topology;


    public class SqlServerClientContext :
        SqlClientContext
    {
        readonly Guid _consumerId;
        readonly SqlServerDbConnectionContext _context;
        readonly string _createQueueSql;
        readonly string _createQueueSubscriptionSql;
        readonly string _createTopicSql;
        readonly string _createTopicSubscriptionSql;
        readonly string _deleteMessageSql;
        readonly string _deleteScheduledMessageSql;
        readonly string _moveMessageTypeSql;
        readonly string _publishSql;
        readonly string _purgeQueueSql;
        readonly string _receivePartitionedSql;
        readonly string _receiveSql;
        readonly string _renewMessageLockSql;
        readonly string _sendSql;
        readonly string _touchQueueSql;
        readonly string _unlockSql;
        readonly string _deadLetterMessagesSql;

        public SqlServerClientContext(SqlServerDbConnectionContext context, CancellationToken cancellationToken)
            : base(context, cancellationToken)
        {
            _context = context;
            _consumerId = NewId.NextGuid();

            _createQueueSql = $"{_context.Schema}.CreateQueueV2";
            _createTopicSql = $"{_context.Schema}.CreateTopic";
            _createTopicSubscriptionSql = $"{_context.Schema}.CreateTopicSubscription";
            _createQueueSubscriptionSql = $"{_context.Schema}.CreateQueueSubscription";
            _sendSql = $"{_context.Schema}.SendMessageV2";
            _publishSql = $"{_context.Schema}.PublishMessageV2";
            _purgeQueueSql = $"{_context.Schema}.PurgeQueue";
            _deadLetterMessagesSql = $"{_context.Schema}.DeadLetterMessages";
            _receiveSql = $"{_context.Schema}.FetchMessages";
            _receivePartitionedSql = $"{_context.Schema}.FetchMessagesPartitioned";
            _deleteMessageSql = $"{_context.Schema}.DeleteMessage";
            _renewMessageLockSql = $"{_context.Schema}.RenewMessageLock";
            _touchQueueSql = $"{_context.Schema}.TouchQueue";
            _unlockSql = $"{_context.Schema}.UnlockMessage";
            _moveMessageTypeSql = $"{_context.Schema}.MoveMessage";
            _deleteScheduledMessageSql = $"{_context.Schema}.DeleteScheduledMessage";
        }

        public override async Task<long> CreateQueue(Queue queue)
        {
            var result = await Execute<long>(_createQueueSql, new
            {
                queueName = queue.QueueName,
                autoDelete = (int?)queue.AutoDeleteOnIdle?.TotalSeconds,
                maxDeliveryCount = queue.MaxDeliveryCount
            });

            return result ?? throw new SqlTopologyException("Create queue failed");
        }

        public override async Task<long> CreateTopic(Topic topic)
        {
            var result = await Execute<long>(_createTopicSql, new { topicName = topic.TopicName });

            return result ?? throw new SqlTopologyException("Create topic failed");
        }

        public override async Task<long> CreateTopicSubscription(TopicToTopicSubscription subscription)
        {
            var result = await Execute<long>(_createTopicSubscriptionSql, new
            {
                SourceTopicName = subscription.Source.TopicName,
                DestinationTopicName = subscription.Destination.TopicName,
                SubscriptionType = (int)subscription.SubscriptionType,
                RoutingKey = subscription.RoutingKey ?? "",
                Filter = "{}"
            });

            return result ?? throw new SqlTopologyException("Create topic subscription failed");
        }

        public override async Task<long> CreateQueueSubscription(TopicToQueueSubscription subscription)
        {
            var result = await Execute<long>(_createQueueSubscriptionSql, new
            {
                SourceTopicName = subscription.Source.TopicName,
                DestinationQueueName = subscription.Destination.QueueName,
                SubscriptionType = (int)subscription.SubscriptionType,
                RoutingKey = subscription.RoutingKey ?? "",
                Filter = "{}"
            });

            return result ?? throw new SqlTopologyException("Create queue subscription failed");
        }

        public override async Task<long> PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            var result = await Execute<long>(_purgeQueueSql, new { QueueName = queueName });

            return result ?? throw new SqlTopologyException("Purge queue failed");
        }

        public override async Task<IEnumerable<SqlTransportMessage>> ReceiveMessages(string queueName, SqlReceiveMode mode, int messageLimit,
            int concurrentLimit, TimeSpan lockDuration)
        {
            try
            {
                if (mode == SqlReceiveMode.Normal)
                {
                    return await Query<SqlTransportMessage>(_receiveSql, new
                    {
                        queueName,
                        consumerId = _consumerId,
                        lockId = NewId.NextGuid(),
                        lockDuration = (int)lockDuration.TotalSeconds,
                        fetchCount = messageLimit
                    }).ConfigureAwait(false);
                }

                var ordered = mode switch
                {
                    SqlReceiveMode.PartitionedOrdered => 1,
                    SqlReceiveMode.PartitionedOrderedConcurrent => 1,
                    _ => 0
                };

                return await Query<SqlTransportMessage>(_receivePartitionedSql, new
                {
                    queueName,
                    consumerId = _consumerId,
                    lockId = NewId.NextGuid(),
                    lockDuration = (int)lockDuration.TotalSeconds,
                    fetchCount = messageLimit,
                    concurrentCount = concurrentLimit,
                    ordered
                }).ConfigureAwait(false);
            }
            catch (SqlException exception) when (exception.Number == 1205)
            {
                return [];
            }
        }

        public override Task TouchQueue(string queueName)
        {
            return Execute<long>(_touchQueueSql, new { queueName });
        }

        public override Task<int?> DeadLetterQueue(string queueName, int messageCount)
        {
            return Execute<int>(_deadLetterMessagesSql, new { queueName, messageCount });
        }

        public override Task Send<T>(string queueName, SqlMessageSendContext<T> context)
        {
            IEnumerable<KeyValuePair<string, object>> headers = context.Headers.GetAll().ToList();
            var headersAsJson = headers.Any() ? JsonSerializer.Serialize(headers, SystemTextJsonMessageSerializer.Options) : null;

            Guid? schedulingTokenId = context.Headers.Get<Guid>(MessageHeaders.SchedulingTokenId);
            DateTime? expirationTime = context.TimeToLive.HasValue ? DateTime.UtcNow + context.TimeToLive.Value : null;

            return Execute<long>(_sendSql, new
            {
                entityName = queueName,
                priority = (int)(context.Priority ?? 100),
                transportMessageId = context.TransportMessageId,
                body = context.Body.GetString(),
                binaryBody = default(byte[]?),
                contentType = context.ContentType?.MediaType,
                messageType = string.Join(";", context.SupportedMessageTypes),
                messageId = context.MessageId,
                correlationId = context.CorrelationId,
                conversationId = context.ConversationId,
                requestId = context.RequestId,
                initiatorId = context.InitiatorId,
                sourceAddress = context.SourceAddress,
                destinationAddress = context.DestinationAddress,
                responseAddress = context.ResponseAddress,
                faultAddress = context.FaultAddress,
                sentTime = context.SentTime,
                expirationTime,
                headers = headersAsJson,
                host = HostInfoCache.HostInfoJson,
                partitionKey = context.PartitionKey,
                routingKey = context.RoutingKey,
                delay = (int?)context.Delay?.TotalSeconds,
                schedulingTokenId
            });
        }

        public override Task Publish<T>(string topicName, SqlMessageSendContext<T> context)
        {
            IEnumerable<KeyValuePair<string, object>> headers = context.Headers.GetAll().ToList();
            var headersAsJson = headers.Any() ? JsonSerializer.Serialize(headers, SystemTextJsonMessageSerializer.Options) : null;

            Guid? schedulingTokenId = context.Headers.Get<Guid>(MessageHeaders.SchedulingTokenId);
            DateTime? expirationTime = context.TimeToLive.HasValue ? DateTime.UtcNow + context.TimeToLive.Value : null;

            return Execute<long>(_publishSql, new
            {
                entityName = topicName,
                priority = (int)(context.Priority ?? 100),
                transportMessageId = context.TransportMessageId,
                body = context.Body.GetString(),
                binaryBody = default(byte[]?),
                contentType = context.ContentType?.MediaType,
                messageType = string.Join(";", context.SupportedMessageTypes),
                messageId = context.MessageId,
                correlationId = context.CorrelationId,
                conversationId = context.ConversationId,
                requestId = context.RequestId,
                initiatorId = context.InitiatorId,
                sourceAddress = context.SourceAddress,
                destinationAddress = context.DestinationAddress,
                responseAddress = context.ResponseAddress,
                faultAddress = context.FaultAddress,
                sentTime = context.SentTime,
                expirationTime,
                headers = headersAsJson,
                host = HostInfoCache.HostInfoJson,
                partitionKey = context.PartitionKey,
                routingKey = context.RoutingKey,
                delay = (int?)context.Delay?.TotalSeconds,
                schedulingTokenId
            });
        }

        public override async Task<bool> DeleteMessage(Guid lockId, long messageDeliveryId)
        {
            var result = await Execute<long>(_deleteMessageSql, new
            {
                messageDeliveryId,
                lockId,
            }).ConfigureAwait(false);

            return result == messageDeliveryId;
        }

        public override async Task<bool> DeleteScheduledMessage(Guid tokenId, CancellationToken cancellationToken)
        {
            IEnumerable<SqlTransportMessage> result = await Query<SqlTransportMessage>(_deleteScheduledMessageSql, new
            {
                tokenId,
            }, cancellationToken).ConfigureAwait(false);

            return result.Any();
        }

        public override async Task<bool> MoveMessage(Guid lockId, long messageDeliveryId, string queueName, SqlQueueType queueType, SendHeaders sendHeaders)
        {
            IEnumerable<KeyValuePair<string, object>> headers = sendHeaders.GetAll().ToList();
            var headersAsJson = headers.Any() ? JsonSerializer.Serialize(headers, SystemTextJsonMessageSerializer.Options) : null;

            var result = await Execute<long>(_moveMessageTypeSql, new
            {
                messageDeliveryId,
                lockId,
                queueName,
                queueType,
                headers = headersAsJson
            }).ConfigureAwait(false);

            return result == messageDeliveryId;
        }

        public override async Task<bool> RenewLock(Guid lockId, long messageDeliveryId, TimeSpan duration)
        {
            var result = await Execute<long>(_renewMessageLockSql, new
            {
                messageDeliveryId,
                lockId,
                duration = (int)duration.TotalSeconds
            }).ConfigureAwait(false);

            return result == messageDeliveryId;
        }

        public override async Task<bool> Unlock(Guid lockId, long messageDeliveryId, TimeSpan delay, SendHeaders sendHeaders)
        {
            IEnumerable<KeyValuePair<string, object>> headers = sendHeaders.GetAll().ToList();
            var headersAsJson = headers.Any() ? JsonSerializer.Serialize(headers, SystemTextJsonMessageSerializer.Options) : null;

            var result = await Execute<long>(_unlockSql, new
            {
                messageDeliveryId,
                lockId,
                delay = delay > TimeSpan.Zero ? Math.Max((int)delay.TotalSeconds, 1) : 0,
                headers = headersAsJson
            }).ConfigureAwait(false);

            return result == messageDeliveryId;
        }

        Task<T?> Execute<T>(string functionName, object values)
            where T : struct
        {
            return _context.Query((connection, transaction) => connection
                .ExecuteScalarAsync<T?>(functionName, values, transaction, commandType: CommandType.StoredProcedure), CancellationToken);
        }

        Task<T?> QuerySingle<T>(string functionName, object values)
            where T : class
        {
            return _context.Query((connection, transaction) => connection
                .QuerySingleAsync<T?>(functionName, values, transaction, commandType: CommandType.StoredProcedure), CancellationToken);
        }

        Task<IEnumerable<T>> Query<T>(string functionName, object values)
            where T : class
        {
            return _context.Query((connection, transaction) => connection
                .QueryAsync<T>(functionName, values, transaction, commandType: CommandType.StoredProcedure), CancellationToken);
        }

        Task<IEnumerable<T>> Query<T>(string functionName, object values, CancellationToken cancellationToken)
            where T : class
        {
            return _context.Query((connection, transaction) => connection
                .QueryAsync<T>(functionName, values, transaction, commandType: CommandType.StoredProcedure), cancellationToken);
        }
    }
}
