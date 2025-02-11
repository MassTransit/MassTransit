---

title: ITeeFilter<TContext, TKey>

---

# ITeeFilter\<TContext, TKey\>

Namespace: MassTransit.Middleware

```csharp
public interface ITeeFilter<TContext, TKey> : ITeeFilter<TContext>, IFilter<TContext>, IProbeSite, IPipeConnector<TContext>, IKeyPipeConnector<TKey>
```

#### Type Parameters

`TContext`<br/>

`TKey`<br/>

Implements [ITeeFilter\<TContext\>](../masstransit-middleware/iteefilter-1), [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<TContext\>](../masstransit-middleware/ipipeconnector-1), [IKeyPipeConnector\<TKey\>](../masstransit-middleware/ikeypipeconnector-1)
