---

title: IOutputPipeFilter<TInput, TOutput>

---

# IOutputPipeFilter\<TInput, TOutput\>

Namespace: MassTransit.Middleware

```csharp
public interface IOutputPipeFilter<TInput, TOutput> : IFilter<TInput>, IProbeSite, IPipeConnector<TOutput>, IFilterObserverConnector<TOutput>
```

#### Type Parameters

`TInput`<br/>

`TOutput`<br/>

Implements [IFilter\<TInput\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<TOutput\>](../masstransit-middleware/ipipeconnector-1), [IFilterObserverConnector\<TOutput\>](../../masstransit-abstractions/masstransit/ifilterobserverconnector-1)
