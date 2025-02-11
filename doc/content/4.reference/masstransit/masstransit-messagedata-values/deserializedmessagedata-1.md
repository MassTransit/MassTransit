---

title: DeserializedMessageData<T>

---

# DeserializedMessageData\<T\>

Namespace: MassTransit.MessageData.Values

When a message data property is deserialized, this is used as a placeholder for the actual message
 data accessor which replaces this property value once the message is transformed on the pipeline.

```csharp
public class DeserializedMessageData<T> : MessageData<T>, IMessageData
```

#### Type Parameters

`T`<br/>
The type used to access the message data, valid types include stream, string, and byte[].

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DeserializedMessageData\<T\>](../masstransit-messagedata-values/deserializedmessagedata-1)<br/>
Implements [MessageData\<T\>](../../masstransit-abstractions/masstransit/messagedata-1), [IMessageData](../../masstransit-abstractions/masstransit/imessagedata)

## Properties

### **Address**

```csharp
public Uri Address { get; }
```

#### Property Value

Uri<br/>

### **HasValue**

```csharp
public bool HasValue { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Value**

```csharp
public Task<T> Value { get; }
```

#### Property Value

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Constructors

### **DeserializedMessageData(Uri)**

```csharp
public DeserializedMessageData(Uri address)
```

#### Parameters

`address` Uri<br/>
