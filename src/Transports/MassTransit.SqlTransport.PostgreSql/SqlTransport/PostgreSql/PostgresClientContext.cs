namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Dapper;
    using Npgsql;
    using Serialization;
    using Topology;


    public class PostgresClientContext :
        SqlClientContext
    {
        readonly Guid _consumerId;
        readonly PostgresDbConnectionContext _context;
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
        readonly string _renewLockSql;
        readonly string _sendSql;
        readonly string _touchQueueSql;
        readonly string _unlockSql;
        readonly string _deadLetterMessagesSql;

        public PostgresClientContext(PostgresDbConnectionContext context, CancellationToken cancellationToken)
            : base(context, cancellationToken)
        {
            _context = context;
            _consumerId = NewId.NextGuid();

            _createQueueSubscriptionSql = string.Format(SqlStatements.DbCreateQueueSubscriptionSql, _context.Schema);
            _receiveSql = string.Format(SqlStatements.DbReceiveSql, _context.Schema);
            _receivePartitionedSql = string.Format(SqlStatements.DbReceivePartitionedSql, _context.Schema);
            _sendSql = string.Format(SqlStatements.DbEnqueueSql, _context.Schema);
            _createTopicSubscriptionSql = string.Format(SqlStatements.DbCreateTopicSubscriptionSql, _context.Schema);
            _publishSql = string.Format(SqlStatements.DbPublishSql, _context.Schema);
            _purgeQueueSql = string.Format(SqlStatements.DbPurgeQueueSql, _context.Schema);
            _deadLetterMessagesSql = string.Format(SqlStatements.DbDeadLetterMessagesSql, _context.Schema);
            _createTopicSql = string.Format(SqlStatements.DbCreateTopicSql, _context.Schema);
            _createQueueSql = string.Format(SqlStatements.DbCreateQueueSql, _context.Schema);
            _deleteMessageSql = string.Format(SqlStatements.DbDeleteMessageSql, _context.Schema);
            _deleteScheduledMessageSql = string.Format(SqlStatements.DbDeleteScheduledMessageSql, _context.Schema);
            _moveMessageTypeSql = string.Format(SqlStatements.DbMoveMessageSql, _context.Schema);
            _renewLockSql = string.Format(SqlStatements.DbRenewLockSql, _context.Schema);
            _touchQueueSql = string.Format(SqlStatements.DbTouchQueueSql, _context.Schema);
            _unlockSql = string.Format(SqlStatements.DbUnlockSql, _context.Schema);
        }

        public override Task<long> CreateQueue(Queue queue)
        {
            return _context.Query((x, t) => x.ExecuteScalarAsync<long>(_createQueueSql, new
            {
                queue_name = queue.QueueName,
                auto_delete = (int?)queue.AutoDeleteOnIdle?.TotalSeconds,
                max_delivery_count = queue.MaxDeliveryCount
            }, t), CancellationToken);
        }

        public override Task<long> CreateTopic(Topic topic)
        {
            return _context.Query((x, t) => x.ExecuteScalarAsync<long>(_createTopicSql, new { topic_name = topic.TopicName }), CancellationToken);
        }

        public override Task<long> CreateTopicSubscription(TopicToTopicSubscription subscription)
        {
            return _context.Query((x, t) => x.ExecuteScalarAsync<long>(_createTopicSubscriptionSql, new
            {
                source_topic_name = subscription.Source.TopicName,
                destination_topic_name = subscription.Destination.TopicName,
                type = (int)subscription.SubscriptionType,
                routing_key = subscription.RoutingKey,
                filter = new JsonParameter(null)
            }), CancellationToken);
        }

        public override Task<long> CreateQueueSubscription(TopicToQueueSubscription subscription)
        {
            return _context.Query((x, t) => x.ExecuteScalarAsync<long>(_createQueueSubscriptionSql, new
            {
                source_topic_name = subscription.Source.TopicName,
                destination_queue_name = subscription.Destination.QueueName,
                type = (int)subscription.SubscriptionType,
                routing_key = subscription.RoutingKey,
                filter = new JsonParameter(null)
            }), CancellationToken);
        }

        public override Task<long> PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            return _context.Query((x, t) => x.ExecuteScalarAsync<long>(_purgeQueueSql, new { queue_name = queueName }), CancellationToken);
        }

        public override async Task<IEnumerable<SqlTransportMessage>> ReceiveMessages(string queueName, SqlReceiveMode mode, int messageLimit,
            int concurrentLimit, TimeSpan lockDuration)
        {
            try
            {
                if (mode == SqlReceiveMode.Normal)
                {
                    return await _context.Query((x, t) => x.QueryAsync<SqlTransportMessage>(_receiveSql, new
                    {
                        queue_name = queueName,
                        fetch_consumer_id = _consumerId,
                        fetch_lock_id = NewId.NextGuid(),
                        lock_duration = lockDuration,
                        fetch_count = messageLimit
                    }), CancellationToken).ConfigureAwait(false);
                }

                var ordered = mode switch
                {
                    SqlReceiveMode.PartitionedOrdered => 1,
                    SqlReceiveMode.PartitionedOrderedConcurrent => 1,
                    _ => 0
                };

                return await _context.Query((x, t) => x.QueryAsync<SqlTransportMessage>(_receivePartitionedSql, new
                {
                    queue_name = queueName,
                    fetch_consumer_id = _consumerId,
                    fetch_lock_id = NewId.NextGuid(),
                    lock_duration = lockDuration,
                    fetch_count = messageLimit,
                    concurrent_count = concurrentLimit,
                    ordered
                }), CancellationToken).ConfigureAwait(false);
            }
            catch (PostgresException exception) when (exception.ErrorCode == 40001)
            {
                return [];
            }
        }

        public override Task TouchQueue(string queueName)
        {
            return _context.Query((x, t) => x.ExecuteScalarAsync<int?>(_touchQueueSql, new { queue_name = queueName }), CancellationToken);
        }

        public override Task<int?> DeadLetterQueue(string queueName, int messageCount)
        {
            return _context.Query((x, t) => x.ExecuteScalarAsync<int?>(_deadLetterMessagesSql, new
            {
                queue_name = queueName,
                message_count = messageCount
            }), CancellationToken);
        }

        public override Task Send<T>(string queueName, SqlMessageSendContext<T> context)
        {
            IEnumerable<KeyValuePair<string, object>> headers = context.Headers.GetAll().ToList();
            var headersAsJson = headers.Any() ? JsonSerializer.Serialize(headers, SystemTextJsonMessageSerializer.Options) : null;

            Guid? schedulingTokenId = context.Headers.Get<Guid>(MessageHeaders.SchedulingTokenId);
            DateTime? expirationTime = context.TimeToLive.HasValue ? DateTime.UtcNow + context.TimeToLive.Value : null;

            return _context.Query((x, t) => x.ExecuteScalarAsync<long?>(_sendSql, new
            {
                entity_name = queueName,
                priority = (int)(context.Priority ?? 100),
                transport_message_id = context.TransportMessageId,
                body = new JsonParameter(context.Body.GetString()),
                binary_body = default(byte[]?),
                content_type = context.ContentType?.MediaType,
                message_type = string.Join(";", context.SupportedMessageTypes),
                message_id = context.MessageId,
                correlation_id = context.CorrelationId,
                conversation_id = context.ConversationId,
                request_id = context.RequestId,
                initiator_id = context.InitiatorId,
                source_address = context.SourceAddress,
                destination_address = context.DestinationAddress,
                response_address = context.ResponseAddress,
                fault_address = context.FaultAddress,
                sent_time = context.SentTime,
                expiration_time = expirationTime,
                headers = new JsonParameter(headersAsJson),
                host = new JsonParameter(HostInfoCache.HostInfoJson),
                partition_key = context.PartitionKey,
                routing_key = context.RoutingKey,
                delay = context.Delay,
                scheduling_token_id = schedulingTokenId
            }), CancellationToken);
        }

        public override Task Publish<T>(string topicName, SqlMessageSendContext<T> context)
        {
            IEnumerable<KeyValuePair<string, object>> headers = context.Headers.GetAll().ToList();
            var headersAsJson = headers.Any() ? JsonSerializer.Serialize(headers, SystemTextJsonMessageSerializer.Options) : null;

            Guid? schedulingTokenId = context.Headers.Get<Guid>(MessageHeaders.SchedulingTokenId);
            DateTime? expirationTime = context.TimeToLive.HasValue ? DateTime.UtcNow + context.TimeToLive.Value : null;

            return _context.Query((x, t) => x.ExecuteScalarAsync<long?>(_publishSql, new
            {
                entity_name = topicName,
                priority = (int)(context.Priority ?? 100),
                transport_message_id = context.TransportMessageId,
                body = new JsonParameter(context.Body.GetString()),
                binary_body = default(byte[]?),
                content_type = context.ContentType?.MediaType,
                message_type = string.Join(";", context.SupportedMessageTypes),
                message_id = context.MessageId,
                correlation_id = context.CorrelationId,
                conversation_id = context.ConversationId,
                request_id = context.RequestId,
                initiator_id = context.InitiatorId,
                source_address = context.SourceAddress,
                destination_address = context.DestinationAddress,
                response_address = context.ResponseAddress,
                fault_address = context.FaultAddress,
                sent_time = context.SentTime,
                expiration_time = expirationTime,
                headers = new JsonParameter(headersAsJson),
                host = new JsonParameter(HostInfoCache.HostInfoJson),
                partition_key = context.PartitionKey,
                routing_key = context.RoutingKey,
                delay = context.Delay,
                scheduling_token_id = schedulingTokenId
            }), CancellationToken);
        }

        public override async Task<bool> DeleteMessage(Guid lockId, long messageDeliveryId)
        {
            var result = await _context.Query((x, t) => x.ExecuteScalarAsync<long?>(_deleteMessageSql, new
            {
                message_delivery_id = messageDeliveryId,
                lock_id = lockId
            }), CancellationToken);

            return result == messageDeliveryId;
        }

        public override async Task<bool> DeleteScheduledMessage(Guid tokenId, CancellationToken cancellationToken)
        {
            IEnumerable<SqlTransportMessage>? result = await _context.Query((x, t) => x.QueryAsync<SqlTransportMessage>(_deleteScheduledMessageSql, new
            {
                token_id = tokenId,
            }), cancellationToken);

            return result.Any();
        }

        public override async Task<bool> MoveMessage(Guid lockId, long messageDeliveryId, string queueName, SqlQueueType queueType, SendHeaders sendHeaders)
        {
            IEnumerable<KeyValuePair<string, object>> headers = sendHeaders.GetAll().ToList();
            var headersAsJson = headers.Any() ? JsonSerializer.Serialize(headers, SystemTextJsonMessageSerializer.Options) : null;

            var result = await _context.Query((x, t) => x.ExecuteScalarAsync<long?>(_moveMessageTypeSql, new
            {
                message_delivery_id = messageDeliveryId,
                lock_id = lockId,
                queue_name = queueName,
                queue_type = (int)queueType,
                headers = new JsonParameter(headersAsJson),
            }), CancellationToken);

            return result == messageDeliveryId;
        }

        public override async Task<bool> RenewLock(Guid lockId, long messageDeliveryId, TimeSpan duration)
        {
            var result = await _context.Query((x, t) => x.ExecuteScalarAsync<long?>(_renewLockSql, new
            {
                message_delivery_id = messageDeliveryId,
                lock_id = lockId,
                duration
            }), CancellationToken);

            return result == messageDeliveryId;
        }

        public override async Task<bool> Unlock(Guid lockId, long messageDeliveryId, TimeSpan delay, SendHeaders sendHeaders)
        {
            IEnumerable<KeyValuePair<string, object>> headers = sendHeaders.GetAll().ToList();
            var headersAsJson = headers.Any() ? JsonSerializer.Serialize(headers, SystemTextJsonMessageSerializer.Options) : null;

            var result = await _context.Query((x, t) => x.ExecuteScalarAsync<long?>(_unlockSql, new
            {
                message_delivery_id = messageDeliveryId,
                lock_id = lockId,
                delay,
                headers = new JsonParameter(headersAsJson),
            }), CancellationToken);

            return result == messageDeliveryId;
        }
    }
}
