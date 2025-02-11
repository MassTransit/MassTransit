---

title: SystemTextJsonRawSerializerContext

---

# SystemTextJsonRawSerializerContext

Namespace: MassTransit.Serialization

```csharp
public class SystemTextJsonRawSerializerContext : SystemTextJsonSerializerContext, SerializerContext, MessageContext, IObjectDeserializer
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseSerializerContext](../masstransit-serialization/baseserializercontext) → [SystemTextJsonSerializerContext](../masstransit-serialization/systemtextjsonserializercontext) → [SystemTextJsonRawSerializerContext](../masstransit-serialization/systemtextjsonrawserializercontext)<br/>
Implements [SerializerContext](../../masstransit-abstractions/masstransit/serializercontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [IObjectDeserializer](../../masstransit-abstractions/masstransit/iobjectdeserializer)

## Properties

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

```csharp
public Uri SourceAddress { get; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public Uri DestinationAddress { get; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public Headers Headers { get; }
```

#### Property Value

[Headers](../../masstransit-abstractions/masstransit/headers)<br/>

### **Host**

```csharp
public HostInfo Host { get; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

### **SupportedMessageTypes**

```csharp
public String[] SupportedMessageTypes { get; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **SystemTextJsonRawSerializerContext(IObjectDeserializer, JsonSerializerOptions, ContentType, MessageContext, String[], RawSerializerOptions, JsonElement)**

```csharp
public SystemTextJsonRawSerializerContext(IObjectDeserializer objectDeserializer, JsonSerializerOptions options, ContentType contentType, MessageContext messageContext, String[] messageTypes, RawSerializerOptions rawOptions, JsonElement message)
```

#### Parameters

`objectDeserializer` [IObjectDeserializer](../../masstransit-abstractions/masstransit/iobjectdeserializer)<br/>

`options` JsonSerializerOptions<br/>

`contentType` ContentType<br/>

`messageContext` [MessageContext](../../masstransit-abstractions/masstransit/messagecontext)<br/>

`messageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`rawOptions` [RawSerializerOptions](../masstransit/rawserializeroptions)<br/>

`message` JsonElement<br/>

## Methods

### **GetMessageSerializer()**

```csharp
public IMessageSerializer GetMessageSerializer()
```

#### Returns

[IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)<br/>

### **IsSupportedMessageType\<T\>()**

```csharp
public bool IsSupportedMessageType<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsSupportedMessageType(Type)**

```csharp
public bool IsSupportedMessageType(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetMessageSerializer(Object, String[])**

```csharp
public IMessageSerializer GetMessageSerializer(object message, String[] messageTypes)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)<br/>

### **GetMessageSerializer\<T\>(MessageEnvelope, T)**

```csharp
public IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`envelope` [MessageEnvelope](../../masstransit-abstractions/masstransit-serialization/messageenvelope)<br/>

`message` T<br/>

#### Returns

[IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)<br/>
