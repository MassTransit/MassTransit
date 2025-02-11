---

title: FactoryMethodCompensateActivityFactory<TActivity, TLog>

---

# FactoryMethodCompensateActivityFactory\<TActivity, TLog\>

Namespace: MassTransit.Courier

```csharp
public class FactoryMethodCompensateActivityFactory<TActivity, TLog> : ICompensateActivityFactory<TActivity, TLog>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FactoryMethodCompensateActivityFactory\<TActivity, TLog\>](../masstransit-courier/factorymethodcompensateactivityfactory-2)<br/>
Implements [ICompensateActivityFactory\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/icompensateactivityfactory-2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FactoryMethodCompensateActivityFactory(Func\<TLog, TActivity\>)**

```csharp
public FactoryMethodCompensateActivityFactory(Func<TLog, TActivity> compensateFactory)
```

#### Parameters

`compensateFactory` [Func\<TLog, TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
