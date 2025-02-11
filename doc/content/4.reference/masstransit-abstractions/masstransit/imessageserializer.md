---

title: IMessageSerializer

---

# IMessageSerializer

Namespace: MassTransit

A message serializer is responsible for serializing a message. Shocking, I know.

```csharp
public interface IMessageSerializer
```

## Properties

### **ContentType**

```csharp
public abstract ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Methods

### **GetMessageBody\<T\>(SendContext\<T\>)**

Returns a message body, for the serializer, which can be used by the transport to obtain the
 serialized message in the desired format.

```csharp
MessageBody GetMessageBody<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

#### Returns

[MessageBody](../masstransit/messagebody)<br/>
