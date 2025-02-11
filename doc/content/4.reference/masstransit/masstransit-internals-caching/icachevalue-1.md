---

title: ICacheValue<TValue>

---

# ICacheValue\<TValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public interface ICacheValue<TValue> : ICacheValue
```

#### Type Parameters

`TValue`<br/>

Implements [ICacheValue](../masstransit-internals-caching/icachevalue)

## Properties

### **Value**

```csharp
public abstract Task<TValue> Value { get; }
```

#### Property Value

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **GetValue(Func\<IPendingValue\<TValue\>\>)**

Get the node's value, passing a pending value if for some
 reason the node's value has not yet been accepted or has
 expired.

```csharp
Task<TValue> GetValue(Func<IPendingValue<TValue>> pendingValueFactory)
```

#### Parameters

`pendingValueFactory` [Func\<IPendingValue\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
