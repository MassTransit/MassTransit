---

title: Supervisor

---

# Supervisor

Namespace: MassTransit.Middleware

Supervises a set of agents, allowing for graceful Start, Stop, and Ready state management

```csharp
public class Supervisor : Agent, IAgent, ISupervisor
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../masstransit-middleware/agent) → [Supervisor](../masstransit-middleware/supervisor)<br/>
Implements [IAgent](../masstransit/iagent), [ISupervisor](../masstransit/isupervisor)

## Properties

### **PeakActiveCount**

```csharp
public int PeakActiveCount { get; private set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TotalCount**

```csharp
public long TotalCount { get; private set; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Ready**

```csharp
public Task Ready { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed**

```csharp
public Task Completed { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping**

```csharp
public CancellationToken Stopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Stopped**

```csharp
public CancellationToken Stopped { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **Supervisor()**

Creates a Supervisor

```csharp
public Supervisor()
```

## Methods

### **Add(IAgent)**

```csharp
public void Add(IAgent agent)
```

#### Parameters

`agent` [IAgent](../masstransit/iagent)<br/>

### **SetReady()**

```csharp
public void SetReady()
```

### **StopAgent(StopContext)**

```csharp
protected Task StopAgent(StopContext context)
```

#### Parameters

`context` [StopContext](../masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StopSupervisor(StopSupervisorContext)**

```csharp
protected Task StopSupervisor(StopSupervisorContext context)
```

#### Parameters

`context` [StopSupervisorContext](../masstransit/stopsupervisorcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
