---

title: IConsumeScopeContext

---

# IConsumeScopeContext

Namespace: MassTransit.DependencyInjection

```csharp
public interface IConsumeScopeContext : IAsyncDisposable
```

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public abstract ConsumeContext Context { get; }
```

#### Property Value

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>
