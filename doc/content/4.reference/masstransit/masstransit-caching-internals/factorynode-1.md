---

title: FactoryNode<TValue>

---

# FactoryNode\<TValue\>

Namespace: MassTransit.Caching.Internals

A factory node is a temporary node used by an index until the node has
 been resolved.

```csharp
public class FactoryNode<TValue> : INode<TValue>
```

#### Type Parameters

`TValue`<br/>
The value type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FactoryNode\<TValue\>](../masstransit-caching-internals/factorynode-1)<br/>
Implements [INode\<TValue\>](../masstransit-caching/inode-1)

## Properties

### **Value**

```csharp
public Task<TValue> Value { get; }
```

#### Property Value

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **HasValue**

```csharp
public bool HasValue { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsValid**

```csharp
public bool IsValid { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **FactoryNode(INodeValueFactory\<TValue\>)**

```csharp
public FactoryNode(INodeValueFactory<TValue> nodeValueFactory)
```

#### Parameters

`nodeValueFactory` [INodeValueFactory\<TValue\>](../masstransit-caching-internals/inodevaluefactory-1)<br/>

## Methods

### **GetValue(IPendingValue\<TValue\>)**

```csharp
public Task<TValue> GetValue(IPendingValue<TValue> pendingValue)
```

#### Parameters

`pendingValue` [IPendingValue\<TValue\>](../masstransit-caching/ipendingvalue-1)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
