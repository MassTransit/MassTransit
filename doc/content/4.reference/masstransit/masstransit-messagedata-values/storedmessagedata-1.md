---

title: StoredMessageData<T>

---

# StoredMessageData\<T\>

Namespace: MassTransit.MessageData.Values

MessageData that has been stored by the repository, has a valid address, and is ready to
 be serialized.

```csharp
public class StoredMessageData<T> : MessageData<T>, IMessageData
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StoredMessageData\<T\>](../masstransit-messagedata-values/storedmessagedata-1)<br/>
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

### **StoredMessageData(Uri, T)**

```csharp
public StoredMessageData(Uri address, T value)
```

#### Parameters

`address` Uri<br/>

`value` T<br/>
