---

title: CopyBodySerializer

---

# CopyBodySerializer

Namespace: MassTransit.Serialization

Copies the body of the receive context to the send context unmodified

```csharp
public class CopyBodySerializer : IMessageSerializer
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CopyBodySerializer](../masstransit-serialization/copybodyserializer)<br/>
Implements [IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)

## Properties

### **ContentType**

```csharp
public ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Constructors

### **CopyBodySerializer(ContentType, MessageBody)**

```csharp
public CopyBodySerializer(ContentType contentType, MessageBody body)
```

#### Parameters

`contentType` ContentType<br/>

`body` [MessageBody](../../masstransit-abstractions/masstransit/messagebody)<br/>

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
