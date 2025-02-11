---

title: NodeList<T, TNode>

---

# NodeList\<T, TNode\>

Namespace: MassTransit.Internals.GraphValidation

Maintains a list of nodes for a given set of instances of T

```csharp
public class NodeList<T, TNode> : IEnumerable<TNode>, IEnumerable
```

#### Type Parameters

`T`<br/>
The type encapsulated in the node

`TNode`<br/>
The type of node contained in the list

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NodeList\<T, TNode\>](../masstransit-internals-graphvalidation/nodelist-2)<br/>
Implements [IEnumerable\<TNode\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Item**

```csharp
public TNode Item { get; }
```

#### Property Value

TNode<br/>

## Constructors

### **NodeList(Func\<Int32, T, TNode\>, Int32)**

```csharp
public NodeList(Func<int, T, TNode> nodeFactory, int capacity)
```

#### Parameters

`nodeFactory` [Func\<Int32, T, TNode\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`capacity` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **GetEnumerator()**

```csharp
public IEnumerator<TNode> GetEnumerator()
```

#### Returns

[IEnumerator\<TNode\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>

### **Index(T)**

Retrieve the index for a given key

```csharp
public int Index(T key)
```

#### Parameters

`key` T<br/>
The key

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The index
