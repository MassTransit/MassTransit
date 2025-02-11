---

title: NodeValueFactory<TValue>

---

# NodeValueFactory\<TValue\>

Namespace: MassTransit.Caching.Internals

A factory for a node which keeps track of subsequent attempts to create the
 same node, passing through until a valid node is created.

```csharp
public class NodeValueFactory<TValue> : INodeValueFactory<TValue>
```

#### Type Parameters

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NodeValueFactory\<TValue\>](../masstransit-caching-internals/nodevaluefactory-1)<br/>
Implements [INodeValueFactory\<TValue\>](../masstransit-caching-internals/inodevaluefactory-1)

## Properties

### **Value**

```csharp
public Task<TValue> Value { get; }
```

#### Property Value

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Constructors

### **NodeValueFactory(IPendingValue\<TValue\>, Int32)**

Creates a node value factory, with the inital pending value

```csharp
public NodeValueFactory(IPendingValue<TValue> initialPendingValue, int timeoutInMilliseconds)
```

#### Parameters

`initialPendingValue` [IPendingValue\<TValue\>](../masstransit-caching/ipendingvalue-1)<br/>
The value that brought the node to the cache

`timeoutInMilliseconds` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The timeout to wait for additional factories before faulting

## Methods

### **Add(IPendingValue\<TValue\>)**

```csharp
public void Add(IPendingValue<TValue> pendingValue)
```

#### Parameters

`pendingValue` [IPendingValue\<TValue\>](../masstransit-caching/ipendingvalue-1)<br/>

### **CreateValue()**

```csharp
public Task<TValue> CreateValue()
```

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
