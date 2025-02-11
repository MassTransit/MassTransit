---

title: CreatedConsumeScopeContext

---

# CreatedConsumeScopeContext

Namespace: MassTransit.DependencyInjection

```csharp
public class CreatedConsumeScopeContext : IConsumeScopeContext, IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CreatedConsumeScopeContext](../masstransit-dependencyinjection/createdconsumescopecontext)<br/>
Implements [IConsumeScopeContext](../masstransit-dependencyinjection/iconsumescopecontext), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public ConsumeContext Context { get; }
```

#### Property Value

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

## Constructors

### **CreatedConsumeScopeContext(IServiceScope, ConsumeContext, IDisposable)**

```csharp
public CreatedConsumeScopeContext(IServiceScope scope, ConsumeContext context, IDisposable disposable)
```

#### Parameters

`scope` IServiceScope<br/>

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`disposable` [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
