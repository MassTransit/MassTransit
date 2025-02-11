---

title: ScopeExecuteActivityFactory<TActivity, TArguments>

---

# ScopeExecuteActivityFactory\<TActivity, TArguments\>

Namespace: MassTransit.DependencyInjection

A factory to create an activity from Autofac, that manages the lifetime scope of the activity

```csharp
public class ScopeExecuteActivityFactory<TActivity, TArguments> : IExecuteActivityFactory<TActivity, TArguments>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopeExecuteActivityFactory\<TActivity, TArguments\>](../masstransit-dependencyinjection/scopeexecuteactivityfactory-2)<br/>
Implements [IExecuteActivityFactory\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityfactory-2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScopeExecuteActivityFactory(IExecuteActivityScopeProvider\<TActivity, TArguments\>)**

```csharp
public ScopeExecuteActivityFactory(IExecuteActivityScopeProvider<TActivity, TArguments> scopeProvider)
```

#### Parameters

`scopeProvider` [IExecuteActivityScopeProvider\<TActivity, TArguments\>](../masstransit-dependencyinjection/iexecuteactivityscopeprovider-2)<br/>

## Methods

### **Execute(ExecuteContext\<TArguments\>, IPipe\<ExecuteActivityContext\<TActivity, TArguments\>\>)**

```csharp
public Task Execute(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

`next` [IPipe\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
