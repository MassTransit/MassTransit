---

title: DiscardErrorTransportFilter

---

# DiscardErrorTransportFilter

Namespace: MassTransit.Middleware

Discard the error instead of moving it to the error transport.

```csharp
public class DiscardErrorTransportFilter : IFilter<ExceptionReceiveContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DiscardErrorTransportFilter](../masstransit-middleware/discarderrortransportfilter)<br/>
Implements [IFilter\<ExceptionReceiveContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DiscardErrorTransportFilter()**

```csharp
public DiscardErrorTransportFilter()
```
