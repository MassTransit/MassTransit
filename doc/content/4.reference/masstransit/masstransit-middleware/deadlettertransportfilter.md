---

title: DeadLetterTransportFilter

---

# DeadLetterTransportFilter

Namespace: MassTransit.Middleware

Moves a message received to a transport without any deserialization

```csharp
public class DeadLetterTransportFilter : IFilter<ReceiveContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DeadLetterTransportFilter](../masstransit-middleware/deadlettertransportfilter)<br/>
Implements [IFilter\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DeadLetterTransportFilter()**

```csharp
public DeadLetterTransportFilter()
```
