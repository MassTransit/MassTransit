---

title: RethrowErrorTransportFilter

---

# RethrowErrorTransportFilter

Namespace: MassTransit.Middleware

```csharp
public class RethrowErrorTransportFilter : IFilter<ExceptionReceiveContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RethrowErrorTransportFilter](../masstransit-middleware/rethrowerrortransportfilter)<br/>
Implements [IFilter\<ExceptionReceiveContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **RethrowErrorTransportFilter()**

```csharp
public RethrowErrorTransportFilter()
```

## Methods

### **Send(ExceptionReceiveContext, IPipe\<ExceptionReceiveContext\>)**

```csharp
public Task Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
```

#### Parameters

`context` [ExceptionReceiveContext](../../masstransit-abstractions/masstransit/exceptionreceivecontext)<br/>

`next` [IPipe\<ExceptionReceiveContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
