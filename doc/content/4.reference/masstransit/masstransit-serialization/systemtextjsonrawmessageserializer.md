---

title: SystemTextJsonRawMessageSerializer

---

# SystemTextJsonRawMessageSerializer

Namespace: MassTransit.Serialization

```csharp
public class SystemTextJsonRawMessageSerializer : RawMessageSerializer, IMessageDeserializer, IProbeSite, IMessageSerializer
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RawMessageSerializer](../masstransit-serialization/rawmessageserializer) → [SystemTextJsonRawMessageSerializer](../masstransit-serialization/systemtextjsonrawmessageserializer)<br/>
Implements [IMessageDeserializer](../../masstransit-abstractions/masstransit/imessagedeserializer), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)

## Fields

### **JsonContentType**

```csharp
public static ContentType JsonContentType;
```

## Properties

### **ContentType**

```csharp
public ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Constructors

### **SystemTextJsonRawMessageSerializer(RawSerializerOptions)**

```csharp
public SystemTextJsonRawMessageSerializer(RawSerializerOptions options)
```

#### Parameters

`options` [RawSerializerOptions](../masstransit/rawserializeroptions)<br/>

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
