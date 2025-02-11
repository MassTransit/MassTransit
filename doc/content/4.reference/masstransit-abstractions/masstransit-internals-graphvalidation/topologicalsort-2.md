---

title: TopologicalSort<T, TNode>

---

# TopologicalSort\<T, TNode\>

Namespace: MassTransit.Internals.GraphValidation

```csharp
public class TopologicalSort<T, TNode>
```

#### Type Parameters

`T`<br/>

`TNode`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TopologicalSort\<T, TNode\>](../masstransit-internals-graphvalidation/topologicalsort-2)

## Properties

### **Result**

```csharp
public IEnumerable<TNode> Result { get; }
```

#### Property Value

[IEnumerable\<TNode\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Constructors

### **TopologicalSort(AdjacencyList\<T, TNode\>)**

```csharp
public TopologicalSort(AdjacencyList<T, TNode> list)
```

#### Parameters

`list` [AdjacencyList\<T, TNode\>](../masstransit-internals-graphvalidation/adjacencylist-2)<br/>

### **TopologicalSort(AdjacencyList\<T, TNode\>, T)**

```csharp
public TopologicalSort(AdjacencyList<T, TNode> list, T source)
```

#### Parameters

`list` [AdjacencyList\<T, TNode\>](../masstransit-internals-graphvalidation/adjacencylist-2)<br/>

`source` T<br/>
