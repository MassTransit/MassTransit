---

title: ICacheIndex<TValue>

---

# ICacheIndex\<TValue\>

Namespace: MassTransit.Caching.Internals

```csharp
public interface ICacheIndex<TValue>
```

#### Type Parameters

`TValue`<br/>

## Properties

### **KeyType**

The key type for the index

```csharp
public abstract Type KeyType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Methods

### **Clear()**

Clear the index, removing all nodes, but leaving them unmodified

```csharp
void Clear()
```

### **Add(INode\<TValue\>)**

Adds a node to the index

```csharp
Task<bool> Add(INode<TValue> node)
```

#### Parameters

`node` [INode\<TValue\>](../masstransit-caching/inode-1)<br/>
The node

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
True if the value was added, false if the value already existed in the index

### **TryGetExistingNode(TValue, INode\<TValue\>)**

Check if the value is in the index, and if found, return the node

```csharp
bool TryGetExistingNode(TValue value, out INode<TValue> node)
```

#### Parameters

`value` TValue<br/>
The value

`node` [INode\<TValue\>](../masstransit-caching/inode-1)<br/>
The matching node

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the value was found, otherwise false
