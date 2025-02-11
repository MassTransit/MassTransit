---

title: RescueFilter<TContext, TRescueContext>

---

# RescueFilter\<TContext, TRescueContext\>

Namespace: MassTransit.Middleware

Rescue catches an exception, and if the exception matches the exception filter,
 passes control to the rescue pipe.

```csharp
public class RescueFilter<TContext, TRescueContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>
The context type

`TRescueContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RescueFilter\<TContext, TRescueContext\>](../masstransit-middleware/rescuefilter-2)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **RescueFilter(IPipe\<TRescueContext\>, IExceptionFilter, RescueContextFactory\<TContext, TRescueContext\>)**

```csharp
public RescueFilter(IPipe<TRescueContext> rescuePipe, IExceptionFilter exceptionFilter, RescueContextFactory<TContext, TRescueContext> rescueContextFactory)
```

#### Parameters

`rescuePipe` [IPipe\<TRescueContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`exceptionFilter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`rescueContextFactory` [RescueContextFactory\<TContext, TRescueContext\>](../masstransit-middleware/rescuecontextfactory-2)<br/>
