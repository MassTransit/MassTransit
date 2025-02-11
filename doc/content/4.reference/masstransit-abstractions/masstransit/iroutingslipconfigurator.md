---

title: IRoutingSlipConfigurator

---

# IRoutingSlipConfigurator

Namespace: MassTransit

Configure a message handler, including specifying filters that are executed around
 the handler itself

```csharp
public interface IRoutingSlipConfigurator : IConsumeConfigurator, IPipeConfigurator<ConsumeContext<RoutingSlip>>
```

Implements [IConsumeConfigurator](../masstransit/iconsumeconfigurator), [IPipeConfigurator\<ConsumeContext\<RoutingSlip\>\>](../masstransit/ipipeconfigurator-1)
