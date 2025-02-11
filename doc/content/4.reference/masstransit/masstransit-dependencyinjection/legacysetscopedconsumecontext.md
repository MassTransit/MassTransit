---

title: LegacySetScopedConsumeContext

---

# LegacySetScopedConsumeContext

Namespace: MassTransit.DependencyInjection

```csharp
public class LegacySetScopedConsumeContext : ISetScopedConsumeContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LegacySetScopedConsumeContext](../masstransit-dependencyinjection/legacysetscopedconsumecontext)<br/>
Implements [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)

## Fields

### **Instance**

```csharp
public static ISetScopedConsumeContext Instance;
```

## Methods

### **PushContext(IServiceScope, ConsumeContext)**

```csharp
public IDisposable PushContext(IServiceScope scope, ConsumeContext context)
```

#### Parameters

`scope` IServiceScope<br/>

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>
