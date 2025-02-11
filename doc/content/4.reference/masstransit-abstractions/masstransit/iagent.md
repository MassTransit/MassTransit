---

title: IAgent

---

# IAgent

Namespace: MassTransit

An agent can be supervised, and signals when it has completed

```csharp
public interface IAgent
```

## Properties

### **Ready**

A Task which can be awaited and is completed when the agent is either ready or faulted/canceled

```csharp
public abstract Task Ready { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed**

A Task which is completed when the agent has completed (should never be set to Faulted, per convention)

```csharp
public abstract Task Completed { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping**

The token which indicates if the agent is in the process of stopping (or stopped)

```csharp
public abstract CancellationToken Stopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Stopped**

The token which indicates if the agent is stopped

```csharp
public abstract CancellationToken Stopped { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Stop(StopContext)**

Stop the agent, and any supervised agents under it's control. Any faults related to stopping should
 be returned via this method, and not propagated to the [IAgent.Completed](iagent#completed) Task.

```csharp
Task Stop(StopContext context)
```

#### Parameters

`context` [StopContext](../masstransit/stopcontext)<br/>
The stop context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
