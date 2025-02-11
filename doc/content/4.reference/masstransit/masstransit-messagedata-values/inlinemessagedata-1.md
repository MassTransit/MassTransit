---

title: InlineMessageData<T>

---

# InlineMessageData\<T\>

Namespace: MassTransit.MessageData.Values

```csharp
public class InlineMessageData<T> : MessageData<T>, IMessageData, IInlineMessageData
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InlineMessageData\<T\>](../masstransit-messagedata-values/inlinemessagedata-1)<br/>
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

### **InlineMessageData(Uri, T, IInlineMessageData)**

```csharp
public InlineMessageData(Uri address, T value, IInlineMessageData messageData)
```

#### Parameters

`address` Uri<br/>

`value` T<br/>

`messageData` [IInlineMessageData](../masstransit-messagedata/iinlinemessagedata)<br/>

## Methods

### **Set(IMessageDataReference)**

```csharp
public void Set(IMessageDataReference reference)
```

#### Parameters

`reference` [IMessageDataReference](../masstransit-messagedata/imessagedatareference)<br/>
