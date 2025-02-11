---

title: AgentExtensions

---

# AgentExtensions

Namespace: MassTransit

```csharp
public static class AgentExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AgentExtensions](../masstransit/agentextensions)

## Methods

### **Stop(IAgent, CancellationToken)**

Stop the agent, using the default StopContext

```csharp
public static Task Stop(IAgent agent, CancellationToken cancellationToken)
```

#### Parameters

`agent` [IAgent](../masstransit/iagent)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stop(IAgent, String, CancellationToken)**

Stop the agent, using the default StopContext

```csharp
public static Task Stop(IAgent agent, string reason, CancellationToken cancellationToken)
```

#### Parameters

`agent` [IAgent](../masstransit/iagent)<br/>

`reason` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The reason for stopping the agent

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
