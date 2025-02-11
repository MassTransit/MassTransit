---

title: IReceivedMessage<T>

---

# IReceivedMessage\<T\>

Namespace: MassTransit.Testing

```csharp
public interface IReceivedMessage<T> : IReceivedMessage, IAsyncListElement
```

#### Type Parameters

`T`<br/>

Implements [IReceivedMessage](../masstransit-testing/ireceivedmessage), [IAsyncListElement](../masstransit-testing/iasynclistelement)

## Properties

### **Context**

```csharp
public abstract ConsumeContext<T> Context { get; }
```

#### Property Value

[ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>
