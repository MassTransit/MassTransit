---

title: SystemTextJsonMessageSerializer

---

# SystemTextJsonMessageSerializer

Namespace: MassTransit.Serialization

```csharp
public class SystemTextJsonMessageSerializer : IMessageDeserializer, IProbeSite, IMessageSerializer, IObjectDeserializer
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemTextJsonMessageSerializer](../masstransit-serialization/systemtextjsonmessageserializer)<br/>
Implements [IMessageDeserializer](../../masstransit-abstractions/masstransit/imessagedeserializer), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer), [IObjectDeserializer](../../masstransit-abstractions/masstransit/iobjectdeserializer)

## Fields

### **JsonContentType**

```csharp
public static ContentType JsonContentType;
```

### **Options**

```csharp
public static JsonSerializerOptions Options;
```

### **Instance**

```csharp
public static SystemTextJsonMessageSerializer Instance;
```

## Properties

### **ContentType**

```csharp
public ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Constructors

### **SystemTextJsonMessageSerializer(ContentType)**

```csharp
public SystemTextJsonMessageSerializer(ContentType contentType)
```

#### Parameters

`contentType` ContentType<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Deserialize(ReceiveContext)**

```csharp
public ConsumeContext Deserialize(ReceiveContext receiveContext)
```

#### Parameters

`receiveContext` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

#### Returns

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

### **Deserialize(MessageBody, Headers, Uri)**

```csharp
public SerializerContext Deserialize(MessageBody body, Headers headers, Uri destinationAddress)
```

#### Parameters

`body` [MessageBody](../../masstransit-abstractions/masstransit/messagebody)<br/>

`headers` [Headers](../../masstransit-abstractions/masstransit/headers)<br/>

`destinationAddress` Uri<br/>

#### Returns

[SerializerContext](../../masstransit-abstractions/masstransit/serializercontext)<br/>

### **GetMessageBody(String)**

```csharp
public MessageBody GetMessageBody(string text)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[MessageBody](../../masstransit-abstractions/masstransit/messagebody)<br/>

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

### **DeserializeObject\<T\>(Object, T)**

```csharp
public T DeserializeObject<T>(object value, T defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`defaultValue` T<br/>

#### Returns

T<br/>

### **DeserializeObject\<T\>(Object, Nullable\<T\>)**

```csharp
public Nullable<T> DeserializeObject<T>(object value, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SerializeObject(Object)**

```csharp
public MessageBody SerializeObject(object value)
```

#### Parameters

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[MessageBody](../../masstransit-abstractions/masstransit/messagebody)<br/>
