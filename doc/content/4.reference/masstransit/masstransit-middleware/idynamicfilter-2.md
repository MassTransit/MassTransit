---

title: IDynamicFilter<TInput, TKey>

---

# IDynamicFilter\<TInput, TKey\>

Namespace: MassTransit.Middleware

```csharp
public interface IDynamicFilter<TInput, TKey> : IFilter<TInput>, IProbeSite, IPipeConnector, IKeyPipeConnector<TKey>, IFilterObserverConnector
```

#### Type Parameters

`TInput`<br/>

`TKey`<br/>

Implements [IFilter\<TInput\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector](../masstransit-middleware/ipipeconnector), [IKeyPipeConnector\<TKey\>](../masstransit-middleware/ikeypipeconnector-1), [IFilterObserverConnector](../../masstransit-abstractions/masstransit/ifilterobserverconnector)
