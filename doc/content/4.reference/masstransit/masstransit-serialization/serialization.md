---

title: Serialization

---

# Serialization

Namespace: MassTransit.Serialization

```csharp
public class Serialization : ISerialization, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Serialization](../masstransit-serialization/serialization)<br/>
Implements [ISerialization](../../masstransit-abstractions/masstransit/iserialization), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **DefaultContentType**

```csharp
public ContentType DefaultContentType { get; }
```

#### Property Value

ContentType<br/>

## Constructors

### **Serialization(IEnumerable\<IMessageSerializer\>, ContentType, IEnumerable\<IMessageDeserializer\>, ContentType)**

```csharp
public Serialization(IEnumerable<IMessageSerializer> serializers, ContentType serializerContentType, IEnumerable<IMessageDeserializer> deserializers, ContentType defaultContentType)
```

#### Parameters

`serializers` [IEnumerable\<IMessageSerializer\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`serializerContentType` ContentType<br/>

`deserializers` [IEnumerable\<IMessageDeserializer\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`defaultContentType` ContentType<br/>

## Methods

### **GetMessageSerializer(ContentType)**

```csharp
public IMessageSerializer GetMessageSerializer(ContentType contentType)
```

#### Parameters

`contentType` ContentType<br/>

#### Returns

[IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)<br/>

### **TryGetMessageSerializer(ContentType, IMessageSerializer)**

```csharp
public bool TryGetMessageSerializer(ContentType contentType, out IMessageSerializer serializer)
```

#### Parameters

`contentType` ContentType<br/>

`serializer` [IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetMessageDeserializer(ContentType)**

```csharp
public IMessageDeserializer GetMessageDeserializer(ContentType contentType)
```

#### Parameters

`contentType` ContentType<br/>

#### Returns

[IMessageDeserializer](../../masstransit-abstractions/masstransit/imessagedeserializer)<br/>

### **TryGetMessageDeserializer(ContentType, IMessageDeserializer)**

```csharp
public bool TryGetMessageDeserializer(ContentType contentType, out IMessageDeserializer deserializer)
```

#### Parameters

`contentType` ContentType<br/>

`deserializer` [IMessageDeserializer](../../masstransit-abstractions/masstransit/imessagedeserializer)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
