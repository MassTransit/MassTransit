---

title: AdjacencyList<T, TNode>

---

# AdjacencyList\<T, TNode\>

Namespace: MassTransit.Internals.GraphValidation

```csharp
public class AdjacencyList<T, TNode>
```

#### Type Parameters

`T`<br/>

`TNode`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AdjacencyList\<T, TNode\>](../masstransit-internals-graphvalidation/adjacencylist-2)

## Properties

### **SourceNodes**

```csharp
public IEnumerable<TNode> SourceNodes { get; }
```

#### Property Value

[IEnumerable\<TNode\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Constructors

### **AdjacencyList(Func\<Int32, T, TNode\>, Int32)**

```csharp
public AdjacencyList(Func<int, T, TNode> nodeFactory, int capacity)
```

#### Parameters

`nodeFactory` [Func\<Int32, T, TNode\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

`capacity` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **GetEdges(TNode)**

```csharp
public HashSet<Edge<T, TNode>> GetEdges(TNode index)
```

#### Parameters

`index` TNode<br/>

#### Returns

[HashSet\<Edge\<T, TNode\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1)<br/>

### **AddEdge(T, T, Int32)**

```csharp
public void AddEdge(T source, T target, int weight)
```

#### Parameters

`source` T<br/>

`target` T<br/>

`weight` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **GetNode(T)**

```csharp
public TNode GetNode(T key)
```

#### Parameters

`key` T<br/>

#### Returns

TNode<br/>
