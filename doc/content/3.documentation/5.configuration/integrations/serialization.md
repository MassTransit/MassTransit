---
navigation.title: Serialization
---

# Message Serialization

In MassTransit, developers specify types for messages. MassTransit's serializers then perform the hard work of converting the types to the serializer format (
such as JSON, XML, BSON, etc.) and then back again.

To interoperate with other languages and platforms, the message structure is important.

## Message Envelope

MassTransit encapsulates messages in an envelope before they are serialized. An example JSON message envelope is shown below:

```json
{
    "messageId": "181c0000-6393-3630-36a4-08daf4e7c6da",
    "requestId": "ef375b18-69ee-4a9e-b5ec-44ee1177a27e",
    "correlationId": null,
    "conversationId": null,
    "initiatorId": null,
    "sourceAddress": "rabbitmq://localhost/source",
    "destinationAddress": "rabbitmq://localhost/destination",
    "responseAddress": "rabbitmq://localhost/response",
    "faultAddress": "rabbitmq://localhost/fault",
    "messageType": [
        "urn:message:Company.Project:SubmitOrder"
    ],
    "message": {
        "orderId": "181c0000-6393-3630-36a4-08daf4e7c6da",
        "timestamp": "2023-01-12T21:55:53.714Z"
    },
    "expirationTime": null,
    "sentTime": "2023-01-12T21:55:53.715882Z",
    "headers": {
        "Application-Header": "SomeValue"
    },
    "host": {
        "machineName": "MyComputer",
        "processName": "dotnet",
        "processId": 427,
        "assembly": "TestProject",
        "assemblyVersion": "2.11.1.93",
        "frameworkVersion": "6.0.7",
        "massTransitVersion": "8.0.10.0",
        "operatingSystemVersion": "Unix 12.6.2"
    }
}
```

| Property           |   Type   | Notes       | Set |
|:-------------------|:--------:|:------------|:---:|
| messageId          |   Guid   | Recommended |  Y  |
| correlationId      |   Guid   | Optional    |     |
| requestId          |   Guid   | Situational |  R  |
| initiatorId        |   Guid   | Optional    |     |
| conversationId     |   Guid   | Optional    |  Y  |
| sourceAddress      |   Uri    | Optional    |  Y  |
| destinationAddress |   Uri    | Optional    |  Y  |
| responseAddress    |   Uri    | Situational |  R  |
| faultAddress       |   Uri    | Optional    |     |
| expirationTime     | ISO-8601 | Situational |  S  |
| sentTime           | ISO-8601 | Optional    |  Y  |
| messageType        | Urn\[\]  | Required    |  Y  |

> Set indicates whether the property is automatically set by MassTransit when producing messages. _Yes_, _Requests_ only, or _Situational_.

### Message Type

MassTransit stores the supported .NET types for a message as an array of URNs, which include the namespace and name of the message type. All interfaces and
superclasses of the message type are included. The namespace and name are formatted as shown below.

`urn:message:Namespace:TypeName`

A few examples of valid message types:

```text
urn:message:MyProject.Messages:UpdateAccount
urn:message:MyProject.Messages.Events:AccountUpdated
urn:message:MyProject:ChangeAccount
urn:message:MyProject.AccountService:MyService+AccountUpdatedEvent
```

> The last one is a nested class, as indicated by the '+' symbol.

## Raw JSON

When using a serializer that doesn't wrap the message in an envelope (_application/json_), the above message would be reduced to the simple JSON below.

```json
{
    "orderId": "181c0000-6393-3630-36a4-08daf4e7c6da",
    "timestamp": "2023-01-12T21:55:53.714Z"
}
```

If the _RawSerializerOptions.AddTransportHeaders_ option is specified when configuring a raw JSON serializer, the following transport headers will be set if the header value is present.

| Header Name          |   Type   | Notes                                                     |
|:---------------------|:--------:|:----------------------------------------------------------|
| MessageId            |   Guid   |                                                           |
| CorrelationId        |   Guid   |                                                           |
| RequestId            |   Guid   |                                                           |
| MT-InitiatorId       |   Guid   |                                                           |
| ConversationId       |   Guid   |                                                           |
| MT-Source-Address    |   Uri    |                                                           |
| MT-Response-Address  |   Uri    |                                                           |
| MT-Fault-Address     |   Uri    |                                                           |
| MT-MessageType       | Urn\[\]  | Multiple message types separated by ;                     |
| MT-Host-Info         |  string  | JSON serialized host info                                 |
| MT-OriginalMessageId |   Guid   | For redelivered messages with a newly generated MessageId |

MassTransit provides several options when dealing with raw JSON messages. The options can be specified on the _UseRawJsonSerializer_ method._RawSerializerOptions_ includes the following flags:

| Option              | Value | Default | Notes                                                        |
|:--------------------|:-----:|:-------:|:-------------------------------------------------------------|
| AnyMessageType      |   1   |    Y    | Messages will match any consumed message type                |
| AddTransportHeaders |   2   |    Y    | MassTransit will add the above headers to outbound messages  |
| CopyHeaders         |   4   |    N    | Received message headers will be copied to outbound messages |

In cases where MassTransit is used and raw JSON messages are preferred, the non-default options are recommended.

```csharp
cfg.UseRawJsonSerializer(RawSerializerOptions.AddTransportHeaders | RawSerializerOptions.CopyHeaders);
```

## Serializers

MassTransit include support for several commonly used serialization packages.

### System Text Json

MassTransit uses _System.Text.Json_ by default to serialize and deserialize JSON messages.

| Content Type                         | Format                | Configuration Method              |
|:-------------------------------------|:----------------------|:----------------------------------|
| **application/vnd.masstransit+json** | **JSON (w/envelope)** | `UseJsonSerializer` **(default)** |
| application/json                     | JSON                  | `UseRawJsonSerializer`            |

### Newtonsoft

The [MassTransit.Newtonsoft](https://nuget.org/packages/MassTransit.Newtonsoft) package adds the following serializer types. Prior to MassTransit v8, Newtonsoft was the default message serializer.

| Content Type                       | Format              | Configuration Method             |
|:-----------------------------------|:--------------------|:---------------------------------|
| application/vnd.masstransit+json   | JSON (w/envelope)   | `UseNewtonsoftJsonSerializer`    |
| application/json                   | JSON                | `UseNewtonsoftRawJsonSerializer` |
| application/vnd.masstransit+bson   | BSON (w/envelope)   | `UseBsonSerializer`              |
| application/vnd.masstransit+xml    | XML  (w/envelope)   | `UseXmlSerializer`               |
| application/xml                    | XML                 | `UseRawXmlSerializer`            |
| application/vnd.masstransit+aes    | Binary (w/envelope) | `UseEncryptedSerializer`         |
| application/vnd.masstransit.v2+aes | Binary (w/envelope) | `UseEncryptedSerializerV2`       |

