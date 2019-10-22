namespace MassTransit
{
    using System;
    using System.Net.Mime;
    using GreenPipes;


    /// <summary>
    /// The SendContext is used to tweak the send to the endpoint
    /// </summary>
    /// <typeparam name="T">The message type being sent</typeparam>
    public interface SendContext<out T> :
        SendContext
        where T : class
    {
        /// <summary>
        /// The message being sent
        /// </summary>
        T Message { get; }
    }


    /// <summary>
    /// Unlike the old world, the send context is returned from the endpoint and used to configure the message sending.
    /// That way the message is captured by the endpoint and then any configuration is done at the higher level.
    /// </summary>
    public interface SendContext :
        PipeContext
    {
        Uri SourceAddress { get; set; }
        Uri DestinationAddress { get; set; }
        Uri ResponseAddress { get; set; }
        Uri FaultAddress { get; set; }

        Guid? RequestId { get; set; }
        Guid? MessageId { get; set; }
        Guid? CorrelationId { get; set; }

        Guid? ConversationId { get; set; }
        Guid? InitiatorId { get; set; }

        Guid? ScheduledMessageId { get; set; }

        SendHeaders Headers { get; }

        TimeSpan? TimeToLive { get; set; }

        ContentType ContentType { get; set; }

        /// <summary>
        /// True if the message should be persisted to disk to survive a broker restart
        /// </summary>
        bool Durable { get; set; }

        /// <summary>
        /// The serializer to use when serializing the message to the transport
        /// </summary>
        IMessageSerializer Serializer { get; set; }

        /// <summary>
        /// Create a send context proxy with the new message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        SendContext<T> CreateProxy<T>(T message)
            where T : class;
    }
}