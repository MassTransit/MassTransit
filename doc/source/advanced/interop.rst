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

This is a minimal message:

.. sourcecode:: javascript

    {
        "destinationAddress": "rabbitmq://isomorphism/MassTransit.Test.Receiver",
        "headers": {},
        "message": {
            "spoken": "Something wierd is going on!",
            "seqId": 2
        },
        "messageType": [
            "urn:message:MassTransit.Test.Messages:ChatMessage"
        ],
        "retryCount": 0
    }

Which translates to these required properties:

 * message
 * messageType
 * destinationAddress

MessageType is a list of urns. See MessageUrnSpecs for the format. Informally, it's like this::

urn:message:NAMESPACE1.NAMESPACE2:TYPE

'retryCount', 'headers' will be defaulted.
