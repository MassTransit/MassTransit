namespace MassTransit
{
    using System;


    /// <summary>
    /// The message context includes the headers that are transferred with the message
    /// </summary>
    public interface MessageContext
    {
        /// <summary>
        /// The messageId assigned to the message when it was initially Sent. This is different
        /// than the transport MessageId, which is only for the Transport.
        /// </summary>
        Guid? MessageId { get; }

        /// <summary>
        /// If the message is a request, or related to a request, such as a response or a fault,
        /// this contains the requestId.
        /// </summary>
        Guid? RequestId { get; }

        /// <summary>
        /// If the message implements the CorrelatedBy(Guid) interface, this field should be
        /// populated by default to match that value. It can, of course, be overwritten with
        /// something else.
        /// </summary>
        Guid? CorrelationId { get; }

        /// <summary>
        /// The conversationId of the message, which is copied and carried throughout the message
        /// flow by the infrastructure.
        /// </summary>
        Guid? ConversationId { get; }

        /// <summary>
        /// If this message was produced within the context of a previous message, the CorrelationId
        /// of the message is contained in this property. If the message was produced from a saga
        /// instance, the CorrelationId of the saga is used.
        /// </summary>
        Guid? InitiatorId { get; }

        /// <summary>
        /// The expiration time of the message if it is not intended to last forever.
        /// </summary>
        DateTime? ExpirationTime { get; }

        /// <summary>
        /// The address of the message producer that sent the message
        /// </summary>
        Uri? SourceAddress { get; }

        /// <summary>
        /// The destination address of the message
        /// </summary>
        Uri? DestinationAddress { get; }

        /// <summary>
        /// The response address to which responses to the request should be sent
        /// </summary>
        Uri? ResponseAddress { get; }

        /// <summary>
        /// The fault address to which fault events should be sent if the message consumer faults
        /// </summary>
        Uri? FaultAddress { get; }

        /// <summary>
        /// When the message was originally sent
        /// </summary>
        DateTime? SentTime { get; }

        /// <summary>
        /// Additional application-specific headers that are added to the message by the application
        /// or by features within MassTransit, such as when a message is moved to an error queue.
        /// </summary>
        Headers Headers { get; }

        /// <summary>
        /// The host information of the message producer. This may not be present if the message was sent
        /// from an earlier version of MassTransit.
        /// </summary>
        HostInfo Host { get; }
    }
}
