---

title: ISetScopedConsumeContext

---

# ISetScopedConsumeContext

Namespace: MassTransit

```csharp
public interface ISetScopedConsumeContext
```

## Methods

### **PushContext(IServiceScope, ConsumeContext)**

```csharp
IDisposable PushContext(IServiceScope serviceProvider, ConsumeContext context)
```

#### Parameters

`serviceProvider` IServiceScope<br/>

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>
