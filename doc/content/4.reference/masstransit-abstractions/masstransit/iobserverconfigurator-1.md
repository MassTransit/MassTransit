---

title: IObserverConfigurator<TMessage>

---

# IObserverConfigurator\<TMessage\>

Namespace: MassTransit

Configure a message handler, including specifying filters that are executed around
 the handler itself

```csharp
public interface IObserverConfigurator<TMessage> : IConsumeConfigurator, IPipeConfigurator<ConsumeContext<TMessage>>
```

#### Type Parameters

`TMessage`<br/>

Implements [IConsumeConfigurator](../masstransit/iconsumeconfigurator), [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../masstransit/ipipeconfigurator-1)
