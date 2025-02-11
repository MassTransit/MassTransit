---

title: StopSupervisorContext

---

# StopSupervisorContext

Namespace: MassTransit

```csharp
public interface StopSupervisorContext : StopContext, PipeContext
```

Implements [StopContext](../masstransit/stopcontext), [PipeContext](../masstransit/pipecontext)

## Properties

### **Agents**

The agents available when the Stop was initiated

```csharp
public abstract IAgent[] Agents { get; }
```

#### Property Value

[IAgent[]](../masstransit/iagent)<br/>
