---

title: Edge<T, TNode>

---

# Edge\<T, TNode\>

Namespace: MassTransit.Internals.GraphValidation

```csharp
public struct Edge<T, TNode>
```

#### Type Parameters

`T`<br/>

`TNode`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Edge\<T, TNode\>](../masstransit-internals-graphvalidation/edge-2)<br/>
Implements [IComparable\<Edge\<T, TNode\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1)

## Fields

### **Source**

```csharp
public TNode Source;
```

### **Target**

```csharp
public TNode Target;
```

### **Weight**

```csharp
public int Weight;
```

## Constructors

### **Edge(TNode, TNode, Int32)**

```csharp
public Edge(TNode source, TNode target, int weight)
```

#### Parameters

`source` TNode<br/>

`target` TNode<br/>

`weight` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **CompareTo(Edge\<T, TNode\>)**

```csharp
public int CompareTo(Edge<T, TNode> other)
```

#### Parameters

`other` [Edge\<T, TNode\>](../masstransit-internals-graphvalidation/edge-2)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
