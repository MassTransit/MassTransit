---

title: CompensateActivityScopeProvider<TActivity, TLog>

---

# CompensateActivityScopeProvider\<TActivity, TLog\>

Namespace: MassTransit.DependencyInjection

```csharp
public class CompensateActivityScopeProvider<TActivity, TLog> : BaseConsumeScopeProvider, ICompensateActivityScopeProvider<TActivity, TLog>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseConsumeScopeProvider](../masstransit-dependencyinjection/baseconsumescopeprovider) → [CompensateActivityScopeProvider\<TActivity, TLog\>](../masstransit-dependencyinjection/compensateactivityscopeprovider-2)<br/>
Implements [ICompensateActivityScopeProvider\<TActivity, TLog\>](../masstransit-dependencyinjection/icompensateactivityscopeprovider-2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **CompensateActivityScopeProvider(IRegistrationContext)**

```csharp
public CompensateActivityScopeProvider(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **CompensateActivityScopeProvider(IServiceProvider, ISetScopedConsumeContext)**

```csharp
public CompensateActivityScopeProvider(IServiceProvider serviceProvider, ISetScopedConsumeContext setScopedConsumeContext)
```

#### Parameters

`serviceProvider` IServiceProvider<br/>

`setScopedConsumeContext` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

## Methods

### **GetScope(CompensateContext\<TLog\>)**

```csharp
public ValueTask<ICompensateScopeContext<TLog>> GetScope(CompensateContext<TLog> context)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

#### Returns

[ValueTask\<ICompensateScopeContext\<TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

### **GetActivityScope(CompensateContext\<TLog\>)**

```csharp
public ValueTask<ICompensateActivityScopeContext<TActivity, TLog>> GetActivityScope(CompensateContext<TLog> context)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

#### Returns

[ValueTask\<ICompensateActivityScopeContext\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
