---

title: DiscardDeadLetterFilter

---

# DiscardDeadLetterFilter

Namespace: MassTransit.Middleware

Simply ignores/discards the not-consumed message

```csharp
public class DiscardDeadLetterFilter : IFilter<ReceiveContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DiscardDeadLetterFilter](../masstransit-middleware/discarddeadletterfilter)<br/>
Implements [IFilter\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DiscardDeadLetterFilter()**

```csharp
public DiscardDeadLetterFilter()
```
