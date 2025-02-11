---

title: SystemTextJsonBodyMessageSerializer

---

# SystemTextJsonBodyMessageSerializer

Namespace: MassTransit.Serialization

Used to serialize an existing deserialized message when a message is forwarded, scheduled, etc.

```csharp
public class SystemTextJsonBodyMessageSerializer : RawMessageSerializer, IMessageSerializer
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RawMessageSerializer](../masstransit-serialization/rawmessageserializer) → [SystemTextJsonBodyMessageSerializer](../masstransit-serialization/systemtextjsonbodymessageserializer)<br/>
Implements [IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)

## Properties

### **ContentType**

```csharp
public ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Constructors

### **SystemTextJsonBodyMessageSerializer(MessageEnvelope, ContentType, JsonSerializerOptions, String[])**

```csharp
public SystemTextJsonBodyMessageSerializer(MessageEnvelope envelope, ContentType contentType, JsonSerializerOptions options, String[] messageTypes)
```

#### Parameters

`envelope` [MessageEnvelope](../../masstransit-abstractions/masstransit-serialization/messageenvelope)<br/>

`contentType` ContentType<br/>

`options` JsonSerializerOptions<br/>

`messageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SystemTextJsonBodyMessageSerializer(Object, ContentType, JsonSerializerOptions, RawSerializerOptions, String[])**

```csharp
public SystemTextJsonBodyMessageSerializer(object message, ContentType contentType, JsonSerializerOptions options, RawSerializerOptions rawOptions, String[] messageTypes)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`contentType` ContentType<br/>

`options` JsonSerializerOptions<br/>

`rawOptions` [RawSerializerOptions](../masstransit/rawserializeroptions)<br/>

`messageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **GetMessageBody\<T\>(SendContext\<T\>)**

```csharp
public MessageBody GetMessageBody<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[MessageBody](../../masstransit-abstractions/masstransit/messagebody)<br/>

### **Overlay(Object)**

```csharp
public void Overlay(object message)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
