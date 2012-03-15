Interop with MassTransit (Routing RFC)
""""""""""""""""""""""""""""""""""""""

MassTransit's serializers do the main work of formatting the data that goes over the wire. Below is the message format that everything is mapped to/from:

.. sourcecode:: csharp

    string RequestId
    string ConversationId
    string CorrelationId
    string DestinationAddress
    DateTime? ExpirationTime
    string FaultAddress
    IDictionary<string, string> Headers
    object Message
    string MessageId
    IList<string> MessageType
    string Network
    string ResponseAddress
    int RetryCount
    string SourceAddress

These are compulsory for a message to be delivered:

 * Message
 * MessageType
 * SourceAddress
 
MessageType is a list of urns. See MessageUrnSpecs for the format. Informally, it's like this::

urn:message:NAMESPACE1.NAMESPACE2:TYPE
