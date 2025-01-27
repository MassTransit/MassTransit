---
navigation.title: Serialization
---

# Message Serialization

MassTransit uses _types_ for [messages](/documentation/concepts/messages). Serializers are used convert those types into their respective format (such as JSON, XML, BSON, etc.), which is sent to the message broker. A corresponding deserializer is then used to convert the serialized format back into a type.

By default, MassTransit uses _System.Text.Json_ to serialize and deserialize messages using JSON.

## Supported Serializers

MassTransit include support for several commonly used serialization packages.

### System.Text.Json

_System.Text.Json_ is the default message serializer/deserializer.

| Content Type                         | Format                | Configuration Method              |
|:-------------------------------------|:----------------------|:----------------------------------|
| **application/vnd.masstransit+json** | **JSON (w/envelope)** | `UseJsonSerializer` **(default)** |
| application/json                     | JSON                  | `UseRawJsonSerializer`            |

#### Customize the Serializer Support

```csharp
services.AddMassTransit(cfg =>
{
    cfg.Using[Broker](broker => 
    {
        broker.ConfigureJsonSerializerOptions(options =>
        {
            // customize the JsonSerializerOptions here
            return options;
        });
    })
    
})
```

### Newtonsoft

The [MassTransit.Newtonsoft](https://nuget.org/packages/MassTransit.Newtonsoft) package adds the serialization formats listed below. 

> In MassTransit versions before v8, Newtonsoft was the default message serializer/deserializer.

| Content Type                       | Format              | Configuration Method             |
|:-----------------------------------|:--------------------|:---------------------------------|
| application/vnd.masstransit+json   | JSON (w/envelope)   | `UseNewtonsoftJsonSerializer`    |
| application/json                   | JSON                | `UseNewtonsoftRawJsonSerializer` |
| application/vnd.masstransit+bson   | BSON (w/envelope)   | `UseBsonSerializer`              |
| application/vnd.masstransit+xml    | XML  (w/envelope)   | `UseXmlSerializer`               |
| application/xml                    | XML                 | `UseRawXmlSerializer`            |
| application/vnd.masstransit+aes    | Binary (w/envelope) | `UseEncryptedSerializer`         |
| application/vnd.masstransit.v2+aes | Binary (w/envelope) | `UseEncryptedSerializerV2`       |

### MessagePack

The [MassTransit.MessagePack](https://nuget.org/packages/MassTransit.MessagePack) package adds the serialization formats listed below. 

| Content Type                        | Format                   | Configuration Method       |
|:------------------------------------|:-------------------------|:---------------------------|
| application/vnd.masstransit+msgpack | MessagePack (w/envelope) | `UseMessagePackSerializer` |


## Message Types

MassTransit stores the supported .NET types for a message as an array of URNs, which include the namespace and name of the message type. All interfaces and
superclasses of the message type are included. The namespace and name are formatted as shown below.

`urn:message:Namespace:TypeName`

A few examples of valid message types:

```
urn:message:MyProject.Messages:UpdateAccount
urn:message:MyProject.Messages.Events:AccountUpdated
urn:message:MyProject:ChangeAccount
urn:message:MyProject.AccountService:MyService+AccountUpdatedEvent
```

> The last one is a nested class, as indicated by the '+' symbol.


### MessageUrn Attribute

_MessageUrn_ is an optional attribute that may be specified on a message type to provide a custom Urn that will be used when the message is published or consumed. The generated Urn will be prefixed with `urn:messages:` by default, however a full Urn may be provided by specifying `useDefaultPrefix: false` in the attribute declaration.

```csharp
[MessageUrn("publish-command")]
public record PublishCommand
{
    // Will generate a urn of: urn:messages:publish-command
}
```

```csharp
[MessageUrn("scheme:publish-command", useDefaultPrefix: false)]
public record PublishCommand
{
    // Will generate a urn of: scheme:publish-command
}
```

## Message Envelope

To interoperate with other languages and platforms, message structure is important. MassTransit encapsulates messages in an envelope before they are serialized. An example JSON message envelope is shown below.

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

## Raw JSON

Consuming messages from other systems where messages may not be produced by MassTransit, raw JSON is commonly used.

When using a serializer that doesn't wrap the message in an envelope (_application/json_), the above message would be reduced to the raw JSON shown below.

```json
{
    "orderId": "181c0000-6393-3630-36a4-08daf4e7c6da",
    "timestamp": "2023-01-12T21:55:53.714Z"
}
```

::callout{type="info"}
#summary
Learn more about using raw JSON messages in this video.

#content
::div
  :video-player{src="https://www.youtube.com/watch?v=xOxSLNeN5CU"}
::
::

### Options

MassTransit provides several options when dealing with raw JSON messages. The options can be specified on the _UseRawJsonSerializer_ method._RawSerializerOptions_ includes the following flags:

| Option              | Value | Default | Notes                                                        |
|:--------------------|:-----:|:-------:|:-------------------------------------------------------------|
| AnyMessageType      |   1   |    N    | Messages will match any consumed message type                |
| AddTransportHeaders |   2   |    Y    | MassTransit will add the above headers to outbound messages  |
| CopyHeaders         |   4   |    Y    | Received message headers will be copied to outbound messages |

In cases where MassTransit is used and raw JSON messages are preferred, the default options are recommended. When integrating with external systems (non-MassTransit generated messages), see [below](#configuration).

### Headers 

When the _RawSerializerOptions.AddTransportHeaders_ option is specified, the following transport headers will be set (if not empty or default).

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

### Configuration

::alert{type="info"}
Serialization customizations for using raw JSON is generally recommended on individual receive endpoints only, vs being globally configured at the bus level.
::

To configure a receive endpoint so that it can _receive_ raw JSON messages, specify the default content type and add the deserializer as shown below. When a raw JSON message is received, it will be delivered to every consumer configured on the receive endpoint.

```csharp
endpointConfigurator.UseRawJsonDeserializer(isDefault: true);
```

If there is no `Message-Type` header that meets MassTransit's requirements (typical for messages not produced by MassTransit), specify the deserializer should return all message types consumed by the consumer. This should only be used when the consumer has only a single `IConsumer<T>` interface.

```csharp
endpointConfigurator.UseRawJsonDeserializer(RawSerializerOptions.All, isDefault: true);
```

If messages produced by consumers on the receive endpoint should also be in the raw JSON format, `UseRawJsonSerializer()` may be used instead.

Setting the default content type tells MassTransit to use the raw JSON deserializer for messages that do not have a recognized `Content-Type` header.

To prevent MassTransit from creating exchanges or topics for the message types consumed on the endpoint, disable consume topology configuration.

```csharp
endpointConfigurator.ConfigureConsumeTopology = false;
```

