---

title: NodeTable<T>

---

# NodeTable\<T\>

Namespace: MassTransit.Internals.GraphValidation

Maintains an index of nodes so that regular ints can be used to execute algorithms
 against objects with int-compare speed vs. .Equals() speed

```csharp
public class NodeTable<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NodeTable\<T\>](../masstransit-internals-graphvalidation/nodetable-1)

## Properties

### **Item**

```csharp
public int Item { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **NodeTable(Int32)**

```csharp
public NodeTable(int capacity)
```

#### Parameters

`capacity` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
