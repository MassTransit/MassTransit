---

title: BytesInlineMessageData

---

# BytesInlineMessageData

Namespace: MassTransit.MessageData.Values

```csharp
public class BytesInlineMessageData : MessageData<Byte[]>, IMessageData, IInlineMessageData
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BytesInlineMessageData](../masstransit-messagedata-values/bytesinlinemessagedata)<br/>
Implements [MessageData\<Byte[]\>](../../masstransit-abstractions/masstransit/messagedata-1), [IMessageData](../../masstransit-abstractions/masstransit/imessagedata), [IInlineMessageData](../masstransit-messagedata/iinlinemessagedata)

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
public Task<Byte[]> Value { get; }
```

#### Property Value

[Task\<Byte[]\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Constructors

### **BytesInlineMessageData(Byte[], Uri)**

```csharp
public BytesInlineMessageData(Byte[] value, Uri address)
```

#### Parameters

`value` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`address` Uri<br/>

## Methods

### **Set(IMessageDataReference)**

```csharp
public void Set(IMessageDataReference reference)
```

#### Parameters

`reference` [IMessageDataReference](../masstransit-messagedata/imessagedatareference)<br/>
