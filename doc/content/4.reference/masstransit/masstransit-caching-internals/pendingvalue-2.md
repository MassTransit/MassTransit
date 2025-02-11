---

title: PendingValue<TKey, TValue>

---

# PendingValue\<TKey, TValue\>

Namespace: MassTransit.Caching.Internals

This is used to store a pending value as a node, which will eventually be published to the indices
 once it resolves. If the pending value faults, it will be removed from the index, unless a subsequent
 factory completes successfully.

```csharp
public class PendingValue<TKey, TValue> : IPendingValue<TValue>
```

#### Type Parameters

`TKey`<br/>
The key type

`TValue`<br/>
The value type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PendingValue\<TKey, TValue\>](../masstransit-caching-internals/pendingvalue-2)<br/>
Implements [IPendingValue\<TValue\>](../masstransit-caching/ipendingvalue-1)

## Properties

### **Value**

```csharp
public Task<TValue> Value { get; }
```

#### Property Value

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Constructors

### **PendingValue(TKey, MissingValueFactory\<TKey, TValue\>)**

```csharp
public PendingValue(TKey key, MissingValueFactory<TKey, TValue> factory)
```

#### Parameters

`key` TKey<br/>

`factory` [MissingValueFactory\<TKey, TValue\>](../masstransit-caching/missingvaluefactory-2)<br/>

## Methods

### **CreateValue()**

```csharp
public Task<TValue> CreateValue()
```

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SetValue(Task\<TValue\>)**

```csharp
public void SetValue(Task<TValue> value)
```

#### Parameters

`value` [Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
