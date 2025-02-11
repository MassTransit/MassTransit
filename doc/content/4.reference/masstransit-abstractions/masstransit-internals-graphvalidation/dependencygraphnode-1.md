---

title: DependencyGraphNode<T>

---

# DependencyGraphNode\<T\>

Namespace: MassTransit.Internals.GraphValidation

```csharp
public class DependencyGraphNode<T> : Node<T>, ITopologicalSortNodeProperties, ITarjanNodeProperties, IComparable<DependencyGraphNode<T>>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Node\<T\>](../masstransit-internals-graphvalidation/node-1) → [DependencyGraphNode\<T\>](../masstransit-internals-graphvalidation/dependencygraphnode-1)<br/>
Implements [ITopologicalSortNodeProperties](../masstransit-internals-graphvalidation/itopologicalsortnodeproperties), [ITarjanNodeProperties](../masstransit-internals-graphvalidation/itarjannodeproperties), [IComparable\<DependencyGraphNode\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1)

## Fields

### **Value**

```csharp
public T Value;
```

## Properties

### **Index**

```csharp
public int Index { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **LowLink**

```csharp
public int LowLink { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Visited**

```csharp
public bool Visited { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **DependencyGraphNode(Int32, T)**

```csharp
public DependencyGraphNode(int index, T value)
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`value` T<br/>
