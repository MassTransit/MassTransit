---

title: Tarjan<T, TNode>

---

# Tarjan\<T, TNode\>

Namespace: MassTransit.Internals.GraphValidation

```csharp
public class Tarjan<T, TNode>
```

#### Type Parameters

`T`<br/>

`TNode`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Tarjan\<T, TNode\>](../masstransit-internals-graphvalidation/tarjan-2)

## Properties

### **Result**

```csharp
public IList<IList<TNode>> Result { get; }
```

#### Property Value

[IList\<IList\<TNode\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

## Constructors

### **Tarjan(AdjacencyList\<T, TNode\>)**

```csharp
public Tarjan(AdjacencyList<T, TNode> list)
```

#### Parameters

`list` [AdjacencyList\<T, TNode\>](../masstransit-internals-graphvalidation/adjacencylist-2)<br/>
