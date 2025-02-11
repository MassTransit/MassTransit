---

title: IOutputPipeFilter<TInput, TOutput, TKey>

---

# IOutputPipeFilter\<TInput, TOutput, TKey\>

Namespace: MassTransit.Middleware

```csharp
public interface IOutputPipeFilter<TInput, TOutput, TKey> : IOutputPipeFilter<TInput, TOutput>, IFilter<TInput>, IProbeSite, IPipeConnector<TOutput>, IFilterObserverConnector<TOutput>, IKeyPipeConnector<TKey>
```

#### Type Parameters

`TInput`<br/>

`TOutput`<br/>

`TKey`<br/>

Implements [IOutputPipeFilter\<TInput, TOutput\>](../masstransit-middleware/ioutputpipefilter-2), [IFilter\<TInput\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<TOutput\>](../masstransit-middleware/ipipeconnector-1), [IFilterObserverConnector\<TOutput\>](../../masstransit-abstractions/masstransit/ifilterobserverconnector-1), [IKeyPipeConnector\<TKey\>](../masstransit-middleware/ikeypipeconnector-1)
