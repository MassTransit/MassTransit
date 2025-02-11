---

title: BytesInlineMessageData<T>

---

# BytesInlineMessageData\<T\>

Namespace: MassTransit.MessageData.Values

```csharp
public class BytesInlineMessageData<T> : MessageData<T>, IMessageData, IInlineMessageData
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BytesInlineMessageData\<T\>](../masstransit-messagedata-values/bytesinlinemessagedata-1)<br/>
Implements [MessageData\<T\>](../../masstransit-abstractions/masstransit/messagedata-1), [IMessageData](../../masstransit-abstractions/masstransit/imessagedata), [IInlineMessageData](../masstransit-messagedata/iinlinemessagedata)

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

### **BytesInlineMessageData(IMessageDataConverter\<T\>, Byte[], Uri)**

```csharp
public BytesInlineMessageData(IMessageDataConverter<T> converter, Byte[] value, Uri address)
```

#### Parameters

`converter` [IMessageDataConverter\<T\>](../masstransit-metadata/imessagedataconverter-1)<br/>

`value` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`address` Uri<br/>

## Methods

### **Set(IMessageDataReference)**

```csharp
public void Set(IMessageDataReference reference)
```

#### Parameters

`reference` [IMessageDataReference](../masstransit-messagedata/imessagedatareference)<br/>
