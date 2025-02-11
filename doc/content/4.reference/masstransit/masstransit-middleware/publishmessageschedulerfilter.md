---

title: PublishMessageSchedulerFilter

---

# PublishMessageSchedulerFilter

Namespace: MassTransit.Middleware

Adds the scheduler to the consume context, so that it can be used for message redelivery

```csharp
public class PublishMessageSchedulerFilter : IFilter<ConsumeContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishMessageSchedulerFilter](../masstransit-middleware/publishmessageschedulerfilter)<br/>
Implements [IFilter\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **PublishMessageSchedulerFilter()**

```csharp
public PublishMessageSchedulerFilter()
```
