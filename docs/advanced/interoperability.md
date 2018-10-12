# Interoperability

In MassTransit, developers specify types for messages. MassTransit's serializers then perform the hard work of converting the types to the serializer format (such as JSON, XML, BSON, etc.) and then back again.

To interoperate with other languages and platforms, the message structure is important.

## Content type

To support custom message types, MassTransit uses a transport-level header to specify the message format. MassTransit simultaneously supports the following message formats on a single transport.

- json (application/vnd.masstransit+json)
- bson (application/vnd.masstransit+bson)
- xml  (application/vnd.masstransit+xml)

If you enable encryption:

- aes  (application/vnd.masstransit+aes)
- aes  (application/vnd.masstransit.v2+aes)

If you configure the binary serializer:

- binary (application/vnd.masstransit+binary)

Register custom types would during endpoint/bus configuration.

## JSON/BSON/XML

MassTransit uses a message envelope to encapsulate the built-in message headers, as well as the message payload. The envelope properties on the wire include:

```csharp
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
```

The *Id* values should be convertible to a GUID/UUID or they will fail. All are optional, but MessageId should be present at a minimum.

The *Address* values should be convertible to a URI that is a valid MassTransit endpoint address.

The *MessageType* entries should be URNs, which are convertible to .NET types. MassTransit defines the format of the URN in the following structure:

```
urn:message:Namespace:TypeName
```

The *Host* is an internal data type, but is a set of strings that define the host that produced the message.

```csharp
string MachineName
string ProcessName
int ProcessId
string Assembly
string AssemblyVersion
string FrameworkVersion
string MassTransitVersion
string OperatingSystemVersion
```

Examples include:

```text
urn:message:MyProject.Messages:UpdateAccount
urn:message:MyProject.Messages.Events:AccountUpdated
urn:message:MyProject:ChangeAccount
urn:message:MyProject.AccountService:MyService+AccountUpdatedEvent
```

The last one is a nested class, as indicated by the '+' symbol.

### Example message

This is a minimal message:

```json
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
```
