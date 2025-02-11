---

title: ISupervisor

---

# ISupervisor

Namespace: MassTransit

A supervisor with a set of agents (a supervisor is also an agent)

```csharp
public interface ISupervisor : IAgent
```

Implements [IAgent](../masstransit/iagent)

## Properties

### **PeakActiveCount**

The peak number of agents active at the same time

```csharp
public abstract int PeakActiveCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TotalCount**

The total number of agents that were added to the supervisor

```csharp
public abstract long TotalCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

## Methods

### **Add(IAgent)**

Add an Agent to the Supervisor

```csharp
void Add(IAgent agent)
```

#### Parameters

`agent` [IAgent](../masstransit/iagent)<br/>
The agent
