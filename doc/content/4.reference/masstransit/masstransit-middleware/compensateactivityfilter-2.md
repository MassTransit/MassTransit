---

title: CompensateActivityFilter<TActivity, TLog>

---

# CompensateActivityFilter\<TActivity, TLog\>

Namespace: MassTransit.Middleware

Compensates an activity as part of an activity execute host pipe

```csharp
public class CompensateActivityFilter<TActivity, TLog> : IFilter<CompensateActivityContext<TActivity, TLog>>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CompensateActivityFilter\<TActivity, TLog\>](../masstransit-middleware/compensateactivityfilter-2)<br/>
Implements [IFilter\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **CompensateActivityFilter(ActivityObservable)**

```csharp
public CompensateActivityFilter(ActivityObservable observers)
```

#### Parameters

`observers` [ActivityObservable](../../masstransit-abstractions/masstransit-observables/activityobservable)<br/>

## Methods

### **Send(CompensateActivityContext\<TActivity, TLog\>, IPipe\<CompensateActivityContext\<TActivity, TLog\>\>)**

```csharp
public Task Send(CompensateActivityContext<TActivity, TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
```

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/compensateactivitycontext-2)<br/>

`next` [IPipe\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
