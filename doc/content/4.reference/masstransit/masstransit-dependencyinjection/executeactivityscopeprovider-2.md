---

title: ExecuteActivityScopeProvider<TActivity, TArguments>

---

# ExecuteActivityScopeProvider\<TActivity, TArguments\>

Namespace: MassTransit.DependencyInjection

```csharp
public class ExecuteActivityScopeProvider<TActivity, TArguments> : BaseConsumeScopeProvider, IExecuteActivityScopeProvider<TActivity, TArguments>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseConsumeScopeProvider](../masstransit-dependencyinjection/baseconsumescopeprovider) → [ExecuteActivityScopeProvider\<TActivity, TArguments\>](../masstransit-dependencyinjection/executeactivityscopeprovider-2)<br/>
Implements [IExecuteActivityScopeProvider\<TActivity, TArguments\>](../masstransit-dependencyinjection/iexecuteactivityscopeprovider-2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ExecuteActivityScopeProvider(IRegistrationContext)**

```csharp
public ExecuteActivityScopeProvider(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **ExecuteActivityScopeProvider(IServiceProvider, ISetScopedConsumeContext)**

```csharp
public ExecuteActivityScopeProvider(IServiceProvider serviceProvider, ISetScopedConsumeContext setScopedConsumeContext)
```

#### Parameters

`serviceProvider` IServiceProvider<br/>

`setScopedConsumeContext` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

## Methods

### **GetScope(ExecuteContext\<TArguments\>)**

```csharp
public ValueTask<IExecuteScopeContext<TArguments>> GetScope(ExecuteContext<TArguments> context)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

#### Returns

[ValueTask\<IExecuteScopeContext\<TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

### **GetActivityScope(ExecuteContext\<TArguments\>)**

```csharp
public ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>> GetActivityScope(ExecuteContext<TArguments> context)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

#### Returns

[ValueTask\<IExecuteActivityScopeContext\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
