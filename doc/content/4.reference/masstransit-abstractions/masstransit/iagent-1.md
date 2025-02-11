---

title: IAgent<TContext>

---

# IAgent\<TContext\>

Namespace: MassTransit

An agent that is also a pipe context source, of the specified context type

```csharp
public interface IAgent<TContext> : IAgent, IPipeContextSource<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Implements [IAgent](../masstransit/iagent), [IPipeContextSource\<TContext\>](../masstransit/ipipecontextsource-1), [IProbeSite](../masstransit/iprobesite)
