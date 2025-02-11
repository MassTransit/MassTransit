---

title: ErrorTransportFilter

---

# ErrorTransportFilter

Namespace: MassTransit.Middleware

In the case of an exception, the message is moved to the destination transport. If the receive had not yet been
 faulted, a fault is generated.

```csharp
public class ErrorTransportFilter : IFilter<ExceptionReceiveContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ErrorTransportFilter](../masstransit-middleware/errortransportfilter)<br/>
Implements [IFilter\<ExceptionReceiveContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ErrorTransportFilter()**

```csharp
public ErrorTransportFilter()
```
