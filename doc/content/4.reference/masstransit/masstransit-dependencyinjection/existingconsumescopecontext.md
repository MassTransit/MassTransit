---

title: ExistingConsumeScopeContext

---

# ExistingConsumeScopeContext

Namespace: MassTransit.DependencyInjection

```csharp
public class ExistingConsumeScopeContext : IConsumeScopeContext, IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExistingConsumeScopeContext](../masstransit-dependencyinjection/existingconsumescopecontext)<br/>
Implements [IConsumeScopeContext](../masstransit-dependencyinjection/iconsumescopecontext), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public ConsumeContext Context { get; }
```

#### Property Value

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

## Constructors

### **ExistingConsumeScopeContext(ConsumeContext, IDisposable)**

```csharp
public ExistingConsumeScopeContext(ConsumeContext context, IDisposable disposable)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`disposable` [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
