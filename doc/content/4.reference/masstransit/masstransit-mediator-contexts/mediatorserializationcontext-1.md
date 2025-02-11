---

title: MediatorSerializationContext<TMessage>

---

# MediatorSerializationContext\<TMessage\>

Namespace: MassTransit.Mediator.Contexts

```csharp
public class MediatorSerializationContext<TMessage> : BaseSerializerContext, SerializerContext, MessageContext, IObjectDeserializer
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseSerializerContext](../masstransit-serialization/baseserializercontext) → [MediatorSerializationContext\<TMessage\>](../masstransit-mediator-contexts/mediatorserializationcontext-1)<br/>
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

### **MediatorSerializationContext(IObjectDeserializer, MessageContext, TMessage, String[])**

```csharp
public MediatorSerializationContext(IObjectDeserializer deserializer, MessageContext context, TMessage message, String[] supportedMessageTypes)
```

#### Parameters

`deserializer` [IObjectDeserializer](../../masstransit-abstractions/masstransit/iobjectdeserializer)<br/>

`context` [MessageContext](../../masstransit-abstractions/masstransit/messagecontext)<br/>

`message` TMessage<br/>

`supportedMessageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **TryGetMessage\<T\>(T)**

```csharp
public bool TryGetMessage<T>(out T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetMessage(Type, Object)**

```csharp
public bool TryGetMessage(Type messageType, out object message)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetMessageSerializer()**

```csharp
public IMessageSerializer GetMessageSerializer()
```

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

### **GetMessageSerializer(Object, String[])**

```csharp
public IMessageSerializer GetMessageSerializer(object message, String[] messageTypes)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)<br/>

### **ToDictionary\<T\>(T)**

```csharp
public Dictionary<string, object> ToDictionary<T>(T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

#### Returns

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>
