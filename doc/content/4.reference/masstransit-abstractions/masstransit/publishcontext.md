---

title: PublishContext

---

# PublishContext

Namespace: MassTransit

```csharp
public interface PublishContext : SendContext, PipeContext
```

Implements [SendContext](../masstransit/sendcontext), [PipeContext](../masstransit/pipecontext)

## Properties

### **Mandatory**

True if the message must be delivered to a subscriber

```csharp
public abstract bool Mandatory { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
