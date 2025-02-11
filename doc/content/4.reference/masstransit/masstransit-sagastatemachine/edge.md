---

title: Edge

---

# Edge

Namespace: MassTransit.SagaStateMachine

```csharp
public class Edge : IEquatable<Edge>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Edge](../masstransit-sagastatemachine/edge)<br/>
Implements [IEquatable\<Edge\>](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **To**

```csharp
public Vertex To { get; }
```

#### Property Value

[Vertex](../masstransit-sagastatemachine/vertex)<br/>

### **From**

```csharp
public Vertex From { get; }
```

#### Property Value

[Vertex](../masstransit-sagastatemachine/vertex)<br/>

### **DebuggerDisplay**

```csharp
public string DebuggerDisplay { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **Edge(Vertex, Vertex, String)**

```csharp
public Edge(Vertex from, Vertex to, string title)
```

#### Parameters

`from` [Vertex](../masstransit-sagastatemachine/vertex)<br/>

`to` [Vertex](../masstransit-sagastatemachine/vertex)<br/>

`title` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Equals(Edge)**

```csharp
public bool Equals(Edge other)
```

#### Parameters

`other` [Edge](../masstransit-sagastatemachine/edge)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
