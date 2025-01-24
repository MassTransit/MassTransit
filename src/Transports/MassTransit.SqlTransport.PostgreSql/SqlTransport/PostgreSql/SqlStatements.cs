namespace MassTransit.SqlTransport.PostgreSql
{
    static class SqlStatements
    {
        public const string DbCreateQueueSql = """SELECT * FROM "{0}".create_queue_v2(@queue_name,@auto_delete,@max_delivery_count)""";
        public const string DbCreateTopicSql = """SELECT * FROM "{0}".create_topic(@topic_name)""";

        public const string DbCreateTopicSubscriptionSql =
            """SELECT * FROM "{0}".create_topic_subscription(@source_topic_name,@destination_topic_name,@type,@routing_key,@filter)""";

        public const string DbCreateQueueSubscriptionSql =
            """SELECT * FROM "{0}".create_queue_subscription(@source_topic_name,@destination_queue_name,@type,@routing_key,@filter)""";

        public const string DbPurgeQueueSql = """SELECT * FROM "{0}".purge_queue(@queue_name)""";

        public const string DbDeadLetterMessagesSql = """SELECT * FROM "{0}".dead_letter_messages(@queue_name,@message_count)""";

        public const string DbEnqueueSql = """
            SELECT * FROM "{0}".send_message_v2(@entity_name,@priority,@transport_message_id,@body,@binary_body,@content_type,
            @message_type,@message_id,@correlation_id,@conversation_id,@request_id,@initiator_id,@source_address,@destination_address,@response_address,@fault_address,
            @sent_time,@expiration_time,@headers,@host,@partition_key,@routing_key,@delay,@scheduling_token_id)
            """;

        public const string DbPublishSql = """
                                           SELECT * FROM "{0}".publish_message_v2(@entity_name,@priority,@transport_message_id,@body,@binary_body,@content_type,
                                           @message_type,@message_id,@correlation_id,@conversation_id,@request_id,@initiator_id,@source_address,@destination_address,@response_address,@fault_address,
                                           @sent_time,@expiration_time,@headers,@host,@partition_key,@routing_key,@delay,@scheduling_token_id)
                                           """;

        public const string DbProcessMetricsSql = """SELECT * FROM "{0}".process_metrics(@row_limit)""";
        public const string DbPurgeTopologySql = """SELECT * FROM "{0}".purge_topology()""";
        public const string DbReceiveSql = """SELECT * FROM "{0}".fetch_messages(@queue_name,@fetch_consumer_id,@fetch_lock_id,@lock_duration,@fetch_count)""";

        public const string DbReceivePartitionedSql =
            """SELECT * FROM "{0}".fetch_messages_partitioned(@queue_name,@fetch_consumer_id,@fetch_lock_id,@lock_duration,@fetch_count,@concurrent_count,@ordered)""";

        public const string DbMoveMessageSql = """SELECT * FROM "{0}".move_message(@message_delivery_id,@lock_id,@queue_name,@queue_type,@headers)""";
        public const string DbDeleteMessageSql = """SELECT * FROM "{0}".delete_message(@message_delivery_id,@lock_id)""";
        public const string DbDeleteScheduledMessageSql = """SELECT * FROM "{0}".delete_scheduled_message(@token_id)""";
        public const string DbRenewLockSql = """SELECT * FROM "{0}".renew_message_lock(@message_delivery_id,@lock_id,@duration)""";
        public const string DbTouchQueueSql = """SELECT * FROM "{0}".touch_queue(@queue_name)""";
        public const string DbUnlockSql = """SELECT * FROM "{0}".unlock_message(@message_delivery_id,@lock_id,@delay,@headers)""";
    }
}
