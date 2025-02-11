---

title: PutMessageData<T>

---

# PutMessageData\<T\>

Namespace: MassTransit.MessageData.Values

Message data that needs to be stored in the repository when the message is sent.

```csharp
public class PutMessageData<T> : MessageData<T>, IMessageData
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PutMessageData\<T\>](../masstransit-messagedata-values/putmessagedata-1)<br/>
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

### **PutMessageData(T, Boolean)**

```csharp
public PutMessageData(T value, bool hasValue)
```

#### Parameters

`value` T<br/>

`hasValue` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
