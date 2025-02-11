---

title: StateMachineGraph

---

# StateMachineGraph

Namespace: MassTransit.SagaStateMachine

```csharp
public class StateMachineGraph
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineGraph](../masstransit-sagastatemachine/statemachinegraph)

## Properties

### **Vertices**

```csharp
public IEnumerable<Vertex> Vertices { get; }
```

#### Property Value

[IEnumerable\<Vertex\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Edges**

```csharp
public IEnumerable<Edge> Edges { get; }
```

#### Property Value

[IEnumerable\<Edge\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Constructors

### **StateMachineGraph(IEnumerable\<Vertex\>, IEnumerable\<Edge\>)**

```csharp
public StateMachineGraph(IEnumerable<Vertex> vertices, IEnumerable<Edge> edges)
```

#### Parameters

`vertices` [IEnumerable\<Vertex\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`edges` [IEnumerable\<Edge\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
