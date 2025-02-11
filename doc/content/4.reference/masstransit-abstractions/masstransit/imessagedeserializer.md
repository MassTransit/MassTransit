---

title: IMessageDeserializer

---

# IMessageDeserializer

Namespace: MassTransit

```csharp
public interface IMessageDeserializer : IProbeSite
```

Implements [IProbeSite](../masstransit/iprobesite)

## Properties

### **ContentType**

```csharp
public abstract ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Methods

### **Deserialize(ReceiveContext)**

```csharp
ConsumeContext Deserialize(ReceiveContext receiveContext)
```

#### Parameters

`receiveContext` [ReceiveContext](../masstransit/receivecontext)<br/>

#### Returns

[ConsumeContext](../masstransit/consumecontext)<br/>

### **Deserialize(MessageBody, Headers, Uri)**

```csharp
SerializerContext Deserialize(MessageBody body, Headers headers, Uri destinationAddress)
```

#### Parameters

`body` [MessageBody](../masstransit/messagebody)<br/>

`headers` [Headers](../masstransit/headers)<br/>

`destinationAddress` Uri<br/>

#### Returns

[SerializerContext](../masstransit/serializercontext)<br/>

### **GetMessageBody(String)**

Returns the appropriate message body for the message deserializer, using the input type

```csharp
MessageBody GetMessageBody(string text)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[MessageBody](../masstransit/messagebody)<br/>
