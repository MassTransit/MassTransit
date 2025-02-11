---

title: IConsumeContextOutputMessageTypeFilter<TMessage>

---

# IConsumeContextOutputMessageTypeFilter\<TMessage\>

Namespace: MassTransit.Middleware

```csharp
public interface IConsumeContextOutputMessageTypeFilter<TMessage> : IFilter<ConsumeContext>, IProbeSite, IPipeConnector<ConsumeContext<TMessage>>, IConsumeMessageObserverConnector<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Implements [IFilter\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<ConsumeContext\<TMessage\>\>](../masstransit-middleware/ipipeconnector-1), [IConsumeMessageObserverConnector\<TMessage\>](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector-1)

## Methods

### **ConnectPipe(Guid, IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
ConnectHandle ConnectPipe(Guid key, IPipe<ConsumeContext<TMessage>> pipe)
```

#### Parameters

`key` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
