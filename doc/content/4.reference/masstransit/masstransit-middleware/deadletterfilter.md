---

title: DeadLetterFilter

---

# DeadLetterFilter

Namespace: MassTransit.Middleware

If a message was neither delivered to a consumer nor caused a fault (which was notified already)
 then this filter will send the message to the dead letter pipe.

```csharp
public class DeadLetterFilter : IFilter<ReceiveContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DeadLetterFilter](../masstransit-middleware/deadletterfilter)<br/>
Implements [IFilter\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DeadLetterFilter(IPipe\<ReceiveContext\>)**

```csharp
public DeadLetterFilter(IPipe<ReceiveContext> deadLetterPipe)
```

#### Parameters

`deadLetterPipe` [IPipe\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
