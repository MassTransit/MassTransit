---

title: GenerateFaultFilter

---

# GenerateFaultFilter

Namespace: MassTransit.Middleware

Generates and publishes a  event for the exception

```csharp
public class GenerateFaultFilter : IFilter<ExceptionReceiveContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GenerateFaultFilter](../masstransit-middleware/generatefaultfilter)<br/>
Implements [IFilter\<ExceptionReceiveContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **GenerateFaultFilter()**

```csharp
public GenerateFaultFilter()
```
