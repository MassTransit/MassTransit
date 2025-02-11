---

title: FactoryMethodActivityFactory<TActivity, TArguments, TLog>

---

# FactoryMethodActivityFactory\<TActivity, TArguments, TLog\>

Namespace: MassTransit.Courier

```csharp
public class FactoryMethodActivityFactory<TActivity, TArguments, TLog> : IActivityFactory<TActivity, TArguments, TLog>, IExecuteActivityFactory<TActivity, TArguments>, IProbeSite, ICompensateActivityFactory<TActivity, TLog>
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FactoryMethodActivityFactory\<TActivity, TArguments, TLog\>](../masstransit-courier/factorymethodactivityfactory-3)<br/>
Implements [IActivityFactory\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivityfactory-3), [IExecuteActivityFactory\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityfactory-2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ICompensateActivityFactory\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/icompensateactivityfactory-2)

## Constructors

### **FactoryMethodActivityFactory(Func\<TArguments, TActivity\>, Func\<TLog, TActivity\>)**

```csharp
public FactoryMethodActivityFactory(Func<TArguments, TActivity> executeFactory, Func<TLog, TActivity> compensateFactory)
```

#### Parameters

`executeFactory` [Func\<TArguments, TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`compensateFactory` [Func\<TLog, TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
