---

title: INodeValueFactory<TValue>

---

# INodeValueFactory\<TValue\>

Namespace: MassTransit.Caching.Internals

Holds a queue of pending values, attemping to resolve them in order until
 one of them completes, and then using the completing value for any pending
 values instead of calling their factory methods.

```csharp
public interface INodeValueFactory<TValue>
```

#### Type Parameters

`TValue`<br/>
The value type

## Properties

### **Value**

Returns the final value of the factory, either completed or faulted

```csharp
public abstract Task<TValue> Value { get; }
```

#### Property Value

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **Add(IPendingValue\<TValue\>)**

Add a pending value to the factory, which will either use a previously
 completed value or become the new factory method for the value.

```csharp
void Add(IPendingValue<TValue> pendingValue)
```

#### Parameters

`pendingValue` [IPendingValue\<TValue\>](../masstransit-caching/ipendingvalue-1)<br/>
The factory method

### **CreateValue()**

Called by the node tracker to create the value, which is then redistributed to the indices.
 Should not be called by another as it's used to resolve the value.

```csharp
Task<TValue> CreateValue()
```

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The ultimate value task, either completed or faulted
