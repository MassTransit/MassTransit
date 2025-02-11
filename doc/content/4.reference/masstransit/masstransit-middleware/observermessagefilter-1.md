---

title: ObserverMessageFilter<TMessage>

---

# ObserverMessageFilter\<TMessage\>

Namespace: MassTransit.Middleware

Consumes a message via a message handler and reports the message as consumed or faulted

```csharp
public class ObserverMessageFilter<TMessage> : IFilter<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ObserverMessageFilter\<TMessage\>](../masstransit-middleware/observermessagefilter-1)<br/>
Implements [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ObserverMessageFilter(IObserver\<ConsumeContext\<TMessage\>\>)**

```csharp
public ObserverMessageFilter(IObserver<ConsumeContext<TMessage>> observer)
```

#### Parameters

`observer` [IObserver\<ConsumeContext\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>
