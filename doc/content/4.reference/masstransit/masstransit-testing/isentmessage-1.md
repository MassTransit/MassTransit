---

title: ISentMessage<TMessage>

---

# ISentMessage\<TMessage\>

Namespace: MassTransit.Testing

```csharp
public interface ISentMessage<TMessage> : ISentMessage, IAsyncListElement
```

#### Type Parameters

`TMessage`<br/>

Implements [ISentMessage](../masstransit-testing/isentmessage), [IAsyncListElement](../masstransit-testing/iasynclistelement)

## Properties

### **Context**

```csharp
public abstract SendContext<TMessage> Context { get; }
```

#### Property Value

[SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>
