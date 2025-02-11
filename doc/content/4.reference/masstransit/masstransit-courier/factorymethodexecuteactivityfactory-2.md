---

title: FactoryMethodExecuteActivityFactory<TActivity, TArguments>

---

# FactoryMethodExecuteActivityFactory\<TActivity, TArguments\>

Namespace: MassTransit.Courier

```csharp
public class FactoryMethodExecuteActivityFactory<TActivity, TArguments> : IExecuteActivityFactory<TActivity, TArguments>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FactoryMethodExecuteActivityFactory\<TActivity, TArguments\>](../masstransit-courier/factorymethodexecuteactivityfactory-2)<br/>
Implements [IExecuteActivityFactory\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityfactory-2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FactoryMethodExecuteActivityFactory(Func\<TArguments, TActivity\>)**

```csharp
public FactoryMethodExecuteActivityFactory(Func<TArguments, TActivity> executeFactory)
```

#### Parameters

`executeFactory` [Func\<TArguments, TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
