---

title: IRedeliveryConfigurator

---

# IRedeliveryConfigurator

Namespace: MassTransit

```csharp
public interface IRedeliveryConfigurator : IRetryConfigurator, IExceptionConfigurator, IRetryObserverConnector
```

Implements [IRetryConfigurator](../masstransit/iretryconfigurator), [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector)

## Properties

### **ReplaceMessageId**

Generate a new MessageId for each redelivered message, replacing the original
 MessageId. This is commonly done when using transport-level de-duplication
 with Azure Service Bus or Amazon SQS.

```csharp
public abstract bool ReplaceMessageId { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
