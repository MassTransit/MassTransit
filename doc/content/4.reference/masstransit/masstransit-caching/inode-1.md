---

title: INode<TValue>

---

# INode\<TValue\>

Namespace: MassTransit.Caching

```csharp
public interface INode<TValue>
```

#### Type Parameters

`TValue`<br/>

## Properties

### **Value**

The cached value

```csharp
public abstract Task<TValue> Value { get; }
```

#### Property Value

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **HasValue**

True if the node has a value, resolved, ready to rock

```csharp
public abstract bool HasValue { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsValid**

True if the node value is invalid

```csharp
public abstract bool IsValid { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **GetValue(IPendingValue\<TValue\>)**

Get the node's value, passing a pending value if for some
 reason the node's value has not yet been accepted or has
 expired.

```csharp
Task<TValue> GetValue(IPendingValue<TValue> pendingValue)
```

#### Parameters

`pendingValue` [IPendingValue\<TValue\>](../masstransit-caching/ipendingvalue-1)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
