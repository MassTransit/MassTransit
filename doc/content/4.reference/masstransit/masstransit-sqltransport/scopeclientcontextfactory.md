---

title: ScopeClientContextFactory

---

# ScopeClientContextFactory

Namespace: MassTransit.SqlTransport

```csharp
public class ScopeClientContextFactory : IPipeContextFactory<ClientContext>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopeClientContextFactory](../masstransit-sqltransport/scopeclientcontextfactory)<br/>
Implements [IPipeContextFactory\<ClientContext\>](../masstransit-agents/ipipecontextfactory-1)

## Constructors

### **ScopeClientContextFactory(IConnectionContextSupervisor)**

```csharp
public ScopeClientContextFactory(IConnectionContextSupervisor connectionContextSupervisor)
```

#### Parameters

`connectionContextSupervisor` [IConnectionContextSupervisor](../masstransit-sqltransport/iconnectioncontextsupervisor)<br/>
