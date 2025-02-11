---

title: IRetryConfigurator

---

# IRetryConfigurator

Namespace: MassTransit

```csharp
public interface IRetryConfigurator : IExceptionConfigurator, IRetryObserverConnector
```

Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector)

## Methods

### **SetRetryPolicy(RetryPolicyFactory)**

```csharp
void SetRetryPolicy(RetryPolicyFactory factory)
```

#### Parameters

`factory` [RetryPolicyFactory](../../masstransit-abstractions/masstransit-configuration/retrypolicyfactory)<br/>
