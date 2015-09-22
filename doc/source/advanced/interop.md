# Interoperability

In MassTransit, developers specify types for messages. MassTransit's serializers then perform the hard work of converting the types to the serializer format (such as JSON, XML, BSON, etc.) and then back again.

In order to interoperate with other languages and platforms, it is important to understand the message structure.

## Content Type

To support custom message types, a transport-level header is used to specify the message format. The following message formats are supported simultaneously on a single transport.

- json (application/vnd.masstransit+json)
- bson (application/vnd.masstransit+bson)
- xml  (application/vnd.masstransit+xml)

If encryption is used:

- aes  (application/vnd.masstransit+aes)

If binary serializer support is configured:

- binary (application/vnd.masstransit+binary)

Additional custom types would be added as a supported deserializer during endpoint/bus configuration.

## JSON/BSON/XML

A message envelope is used to encapsulate the built-in message headers, as well as the message payload. The envelope properties on the wire include:

    string MessageId
    string CorrelationId
    string ConversationId
    string InitiatorId
    string RequestId
    string SourceAddress
    string DestinationAddress
    string ResponseAddress
    string FaultAddress
    DateTime? ExpirationTime
    IDictionary<string, object> Headers
    object Message
    string[] MessageType
    HostInfo Host

The _Id_ values should be convertible to a Guid/Uuid or they will fail. All are optional, but MessageId should be present at a minimum.

The _Address_ values should be convertible to a URI that is a valid MassTransit endpoint address.

The _MessageType_ entries should be URNs, which are convertible to .NET types. The format of the URN is defined by MassTransit, and is in the following structure:

    urn:message:Namespace:TypeName

The _Host_ is an internal data type, but is a set of strings that define the host that produced the message.

    string MachineName
    string ProcessName
    int ProcessId
    string Assembly
    string AssemblyVersion
    string FrameworkVersion
    string MassTransitVersion
    string OperatingSystemVersion

Examples include:

    urn:message:MyProject.Messages:UpdateAccount
    urn:message:MyProject.Messages.Events:AccountUpdated
    urn:message:MyProject:ChangeAccount
    urn:message:MyProject.AccountService:MyService+AccountUpdatedEvent

The last one is a nested class, as indicated by the '+' symbol.

### Example Message

This is a minimal message:

    {
        "destinationAddress": "rabbitmq://localhost/input_queue",
        "headers": {},
        "message": {
            "value": "Some Value",
            "customerId": 27
        },
        "messageType": [
            "urn:message:MassTransit.Tests:ValueMessage"
        ]
    }


## Encrypted Messages

If the encrypted message serializer is used, BSON is used under the hood. The encryption format is AES-256. Assuming the same Key/IV pair are used, an encrypted message should be compatible across the wire.
