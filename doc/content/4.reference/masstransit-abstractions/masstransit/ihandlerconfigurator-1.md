---

title: IHandlerConfigurator<TMessage>

---

# IHandlerConfigurator\<TMessage\>

Namespace: MassTransit

Configure a message handler, including specifying filters that are executed around
 the handler itself

```csharp
public interface IHandlerConfigurator<TMessage> : IConsumeConfigurator, IHandlerConfigurationObserverConnector, IPipeConfigurator<ConsumeContext<TMessage>>
```

#### Type Parameters

`TMessage`<br/>

Implements [IConsumeConfigurator](../masstransit/iconsumeconfigurator), [IHandlerConfigurationObserverConnector](../masstransit/ihandlerconfigurationobserverconnector), [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../masstransit/ipipeconfigurator-1)
