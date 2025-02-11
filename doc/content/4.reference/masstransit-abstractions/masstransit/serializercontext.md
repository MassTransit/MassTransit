---

title: SerializerContext

---

# SerializerContext

Namespace: MassTransit

```csharp
public interface SerializerContext : MessageContext, IObjectDeserializer
```

Implements [MessageContext](../masstransit/messagecontext), [IObjectDeserializer](../masstransit/iobjectdeserializer)

## Properties

### **SupportedMessageTypes**

```csharp
public abstract String[] SupportedMessageTypes { get; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **IsSupportedMessageType\<T\>()**

```csharp
bool IsSupportedMessageType<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsSupportedMessageType(Type)**

```csharp
bool IsSupportedMessageType(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetMessage\<T\>(T)**

```csharp
bool TryGetMessage<T>(out T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetMessage(Type, Object)**

```csharp
bool TryGetMessage(Type messageType, out object message)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetMessageSerializer()**

Returns a message serializer using the deserialized message ContentType, that can be used to
 serialize the message on another [SendContext](../masstransit/sendcontext).

```csharp
IMessageSerializer GetMessageSerializer()
```

#### Returns

[IMessageSerializer](../masstransit/imessageserializer)<br/>

### **GetMessageSerializer\<T\>(MessageEnvelope, T)**

Returns a message serializer using the deserialized message ContentType, that can be used to
 serialize the message on another [SendContext](../masstransit/sendcontext).

```csharp
IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`envelope` [MessageEnvelope](../masstransit-serialization/messageenvelope)<br/>
The message envelope to modify

`message` T<br/>
A message to overlay on top of the existing message, merging the properties together

#### Returns

[IMessageSerializer](../masstransit/imessageserializer)<br/>

### **GetMessageSerializer(Object, String[])**

Returns a message serializer using the deserialized message ContentType, that can be used to
 serialize the message on another [SendContext](../masstransit/sendcontext).

```csharp
IMessageSerializer GetMessageSerializer(object message, String[] messageTypes)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
A message to overlay on top of the existing message, merging the properties together

`messageTypes` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The supported message types

#### Returns

[IMessageSerializer](../masstransit/imessageserializer)<br/>

### **ToDictionary\<T\>(T)**

Converts a message (or really any object) to a dictionary of string, object. This is serializer dependent, since
 JSON serializers use internal objects for object properties, to encapsulate nested properties, etc.

```csharp
Dictionary<string, object> ToDictionary<T>(T message)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`message` T<br/>
The message

#### Returns

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>
