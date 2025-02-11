---

title: HandlerMessageFilter<TMessage>

---

# HandlerMessageFilter\<TMessage\>

Namespace: MassTransit.Middleware

Consumes a message via a message handler and reports the message as consumed or faulted

```csharp
public class HandlerMessageFilter<TMessage> : IFilter<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HandlerMessageFilter\<TMessage\>](../masstransit-middleware/handlermessagefilter-1)<br/>
Implements [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **HandlerMessageFilter(MessageHandler\<TMessage\>)**

```csharp
public HandlerMessageFilter(MessageHandler<TMessage> handler)
```

#### Parameters

`handler` [MessageHandler\<TMessage\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>
