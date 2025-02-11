---

title: IPublishedMessage<T>

---

# IPublishedMessage\<T\>

Namespace: MassTransit.Testing

```csharp
public interface IPublishedMessage<T> : IPublishedMessage, IAsyncListElement
```

#### Type Parameters

`T`<br/>

Implements [IPublishedMessage](../masstransit-testing/ipublishedmessage), [IAsyncListElement](../masstransit-testing/iasynclistelement)

## Properties

### **Context**

```csharp
public abstract PublishContext<T> Context { get; }
```

#### Property Value

[PublishContext\<T\>](../../masstransit-abstractions/masstransit/publishcontext-1)<br/>
