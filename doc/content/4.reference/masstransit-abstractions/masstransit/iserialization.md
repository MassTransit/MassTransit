---

title: ISerialization

---

# ISerialization

Namespace: MassTransit

```csharp
public interface ISerialization : IProbeSite
```

Implements [IProbeSite](../masstransit/iprobesite)

## Properties

### **DefaultContentType**

```csharp
public abstract ContentType DefaultContentType { get; }
```

#### Property Value

ContentType<br/>

## Methods

### **GetMessageSerializer(ContentType)**

```csharp
IMessageSerializer GetMessageSerializer(ContentType contentType)
```

#### Parameters

`contentType` ContentType<br/>

#### Returns

[IMessageSerializer](../masstransit/imessageserializer)<br/>

### **TryGetMessageSerializer(ContentType, IMessageSerializer)**

```csharp
bool TryGetMessageSerializer(ContentType contentType, out IMessageSerializer serializer)
```

#### Parameters

`contentType` ContentType<br/>

`serializer` [IMessageSerializer](../masstransit/imessageserializer)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetMessageDeserializer(ContentType)**

```csharp
IMessageDeserializer GetMessageDeserializer(ContentType contentType)
```

#### Parameters

`contentType` ContentType<br/>

#### Returns

[IMessageDeserializer](../masstransit/imessagedeserializer)<br/>

### **TryGetMessageDeserializer(ContentType, IMessageDeserializer)**

```csharp
bool TryGetMessageDeserializer(ContentType contentType, out IMessageDeserializer deserializer)
```

#### Parameters

`contentType` ContentType<br/>

`deserializer` [IMessageDeserializer](../masstransit/imessagedeserializer)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
