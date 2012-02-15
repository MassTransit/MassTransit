Interop with MassTransit (Routing RFC)
""""""""""""""""""""""""""""""""""""""

MassTransit's serializers do the main work of formatting the data that goes over the wire. Below is the message format that everything is mapped to/from:

.. sourcecode:: csharp

    string RequestId { get; set; }
    string ConversationId { get; set; }
    string CorrelationId { get; set; }
    string DestinationAddress { get; set; }
    DateTime? ExpirationTime { get; set; }
    string FaultAddress { get; set; }
    IDictionary<string, string> Headers { get; set; }
    object Message { get; set; }
    string MessageId { get; set; }
    IList<string> MessageType { get; set; }
    string Network { get; set; }
    string ResponseAddress { get; set; }
    int RetryCount { get; set; }
    string SourceAddress { get; set; }

These are compulsory for a message to be delivered:

 * Message
 * MessageType
 * SourceAddress
 
MessageType is a list of urns. See MessageUrnSpecs for the format. Informally, it's like this::

urn:message:NAMESPACE1.NAMESPACE2:TYPE
