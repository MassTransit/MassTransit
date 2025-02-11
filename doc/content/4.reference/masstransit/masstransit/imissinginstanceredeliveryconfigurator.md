---

title: IMissingInstanceRedeliveryConfigurator

---

# IMissingInstanceRedeliveryConfigurator

Namespace: MassTransit

```csharp
public interface IMissingInstanceRedeliveryConfigurator : IRedeliveryConfigurator, IRetryConfigurator, IExceptionConfigurator, IRetryObserverConnector
```

Implements [IRedeliveryConfigurator](../masstransit/iredeliveryconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector)

## Properties

### **UseMessageScheduler**

Use the message scheduler context instead of the redelivery context (only use when transport-level redelivery is not available)

```csharp
public abstract bool UseMessageScheduler { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
