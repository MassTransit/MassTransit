---

title: SqlConsumerFilter

---

# SqlConsumerFilter

Namespace: MassTransit.SqlTransport.Middleware

A filter that uses the model context to create a basic consumer and connect it to the model

```csharp
public class SqlConsumerFilter : IFilter<ClientContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlConsumerFilter](../masstransit-sqltransport-middleware/sqlconsumerfilter)<br/>
Implements [IFilter\<ClientContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **SqlConsumerFilter(SqlReceiveEndpointContext)**

```csharp
public SqlConsumerFilter(SqlReceiveEndpointContext context)
```

#### Parameters

`context` [SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>
