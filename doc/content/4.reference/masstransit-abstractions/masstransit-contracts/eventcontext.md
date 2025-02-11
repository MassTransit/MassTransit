---

title: EventContext

---

# EventContext

Namespace: MassTransit.Contracts

```csharp
public interface EventContext : PipeContext
```

Implements [PipeContext](../masstransit/pipecontext)

## Properties

### **Timestamp**

The timestamp at which the command was sent

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
