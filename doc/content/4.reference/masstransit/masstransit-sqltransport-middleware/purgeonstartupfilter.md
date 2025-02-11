---

title: PurgeOnStartupFilter

---

# PurgeOnStartupFilter

Namespace: MassTransit.SqlTransport.Middleware

Purges the queue on startup, only once per filter instance

```csharp
public class PurgeOnStartupFilter : IFilter<ClientContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PurgeOnStartupFilter](../masstransit-sqltransport-middleware/purgeonstartupfilter)<br/>
Implements [IFilter\<ClientContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **PurgeOnStartupFilter(String)**

```csharp
public PurgeOnStartupFilter(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
