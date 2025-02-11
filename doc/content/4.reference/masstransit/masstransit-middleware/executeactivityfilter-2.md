---

title: ExecuteActivityFilter<TActivity, TArguments>

---

# ExecuteActivityFilter\<TActivity, TArguments\>

Namespace: MassTransit.Middleware

Executes an activity as part of an activity execute host pipe

```csharp
public class ExecuteActivityFilter<TActivity, TArguments> : IFilter<ExecuteActivityContext<TActivity, TArguments>>, IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityFilter\<TActivity, TArguments\>](../masstransit-middleware/executeactivityfilter-2)<br/>
Implements [IFilter\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ExecuteActivityFilter(ActivityObservable)**

```csharp
public ExecuteActivityFilter(ActivityObservable observers)
```

#### Parameters

`observers` [ActivityObservable](../../masstransit-abstractions/masstransit-observables/activityobservable)<br/>

## Methods

### **Send(ExecuteActivityContext\<TActivity, TArguments\>, IPipe\<ExecuteActivityContext\<TActivity, TArguments\>\>)**

```csharp
public Task Send(ExecuteActivityContext<TActivity, TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
```

#### Parameters

`context` [ExecuteActivityContext\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/executeactivitycontext-2)<br/>

`next` [IPipe\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
