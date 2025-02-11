---

title: EmptyMessageData<T>

---

# EmptyMessageData\<T\>

Namespace: MassTransit.MessageData.Values

```csharp
public class EmptyMessageData<T> : MessageData<T>, IMessageData
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EmptyMessageData\<T\>](../masstransit-messagedata-values/emptymessagedata-1)<br/>
Implements [MessageData\<T\>](../../masstransit-abstractions/masstransit/messagedata-1), [IMessageData](../../masstransit-abstractions/masstransit/imessagedata)

## Fields

### **Instance**

```csharp
public static MessageData<T> Instance;
```

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
