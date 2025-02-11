---

title: Agent

---

# Agent

Namespace: MassTransit.Middleware

An Agent Provocateur that simply exists, out of context

```csharp
public class Agent : IAgent
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../masstransit-middleware/agent)<br/>
Implements [IAgent](../masstransit/iagent)

## Properties

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

### **Agent()**

Creates the Agent

```csharp
public Agent()
```

## Methods

### **Stop(StopContext)**

```csharp
public Task Stop(StopContext context)
```

#### Parameters

`context` [StopContext](../masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StopAgent(StopContext)**

Stops the agent

```csharp
protected Task StopAgent(StopContext context)
```

#### Parameters

`context` [StopContext](../masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SetReady()**

Puts the agent in a ready state, explicitly

```csharp
public void SetReady()
```

### **SetNotReady(Exception)**

Puts the agent in a faulted state where it will never be ready

```csharp
public void SetNotReady(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **SetReady(Task)**

Set the agent ready for duty

```csharp
protected void SetReady(Task readyTask)
```

#### Parameters

`readyTask` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SetCompleted(Task)**

Set the agent Completed for duty

```csharp
protected void SetCompleted(Task completedTask)
```

#### Parameters

`completedTask` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SetFaulted(Task)**

Set the agent faulted, making it dead.

```csharp
protected void SetFaulted(Task task)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
