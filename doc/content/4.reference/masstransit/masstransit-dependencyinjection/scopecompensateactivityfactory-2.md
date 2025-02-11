---

title: ScopeCompensateActivityFactory<TActivity, TLog>

---

# ScopeCompensateActivityFactory\<TActivity, TLog\>

Namespace: MassTransit.DependencyInjection

A factory to create an activity from Autofac, that manages the lifetime scope of the activity

```csharp
public class ScopeCompensateActivityFactory<TActivity, TLog> : ICompensateActivityFactory<TActivity, TLog>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopeCompensateActivityFactory\<TActivity, TLog\>](../masstransit-dependencyinjection/scopecompensateactivityfactory-2)<br/>
Implements [ICompensateActivityFactory\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/icompensateactivityfactory-2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScopeCompensateActivityFactory(ICompensateActivityScopeProvider\<TActivity, TLog\>)**

```csharp
public ScopeCompensateActivityFactory(ICompensateActivityScopeProvider<TActivity, TLog> scopeProvider)
```

#### Parameters

`scopeProvider` [ICompensateActivityScopeProvider\<TActivity, TLog\>](../masstransit-dependencyinjection/icompensateactivityscopeprovider-2)<br/>

## Methods

### **Compensate(CompensateContext\<TLog\>, IPipe\<CompensateActivityContext\<TActivity, TLog\>\>)**

```csharp
public Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

`next` [IPipe\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
