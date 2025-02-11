---

title: StopContext

---

# StopContext

Namespace: MassTransit

The context associated with stopping an agent

```csharp
public interface StopContext : PipeContext
```

Implements [PipeContext](../masstransit/pipecontext)

## Properties

### **Reason**

The reason for stopping

```csharp
public abstract string Reason { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
