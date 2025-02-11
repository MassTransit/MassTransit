---

title: SharedClientContextFactory

---

# SharedClientContextFactory

Namespace: MassTransit.SqlTransport

```csharp
public class SharedClientContextFactory : IPipeContextFactory<ClientContext>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SharedClientContextFactory](../masstransit-sqltransport/sharedclientcontextfactory)<br/>
Implements [IPipeContextFactory\<ClientContext\>](../masstransit-agents/ipipecontextfactory-1)

## Constructors

### **SharedClientContextFactory(IClientContextSupervisor)**

```csharp
public SharedClientContextFactory(IClientContextSupervisor supervisor)
```

#### Parameters

`supervisor` [IClientContextSupervisor](../masstransit-sqltransport/iclientcontextsupervisor)<br/>
