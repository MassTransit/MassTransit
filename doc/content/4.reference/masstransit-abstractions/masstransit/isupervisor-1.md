---

title: ISupervisor<TContext>

---

# ISupervisor\<TContext\>

Namespace: MassTransit

A supervisor that is also a [IPipeContextSource\<TContext\>](../masstransit/ipipecontextsource-1)

```csharp
public interface ISupervisor<TContext> : ISupervisor, IAgent, IAgent<TContext>, IPipeContextSource<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>
The source context type

Implements [ISupervisor](../masstransit/isupervisor), [IAgent](../masstransit/iagent), [IAgent\<TContext\>](../masstransit/iagent-1), [IPipeContextSource\<TContext\>](../masstransit/ipipecontextsource-1), [IProbeSite](../masstransit/iprobesite)
