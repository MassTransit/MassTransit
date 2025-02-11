---

title: IRequestIdTeeFilter<TMessage>

---

# IRequestIdTeeFilter\<TMessage\>

Namespace: MassTransit.Middleware

```csharp
public interface IRequestIdTeeFilter<TMessage> : ITeeFilter<ConsumeContext<TMessage>>, IFilter<ConsumeContext<TMessage>>, IProbeSite, IPipeConnector<ConsumeContext<TMessage>>, IKeyPipeConnector<TMessage, Guid>
```

#### Type Parameters

`TMessage`<br/>

Implements [ITeeFilter\<ConsumeContext\<TMessage\>\>](../masstransit-middleware/iteefilter-1), [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<ConsumeContext\<TMessage\>\>](../masstransit-middleware/ipipeconnector-1), [IKeyPipeConnector\<TMessage, Guid\>](../masstransit-middleware/ikeypipeconnector-2)
