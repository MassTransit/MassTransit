---

title: IConsumerConfigurator

---

# IConsumerConfigurator

Namespace: MassTransit

```csharp
public interface IConsumerConfigurator : IConsumeConfigurator, IConsumerConfigurationObserverConnector
```

Implements [IConsumeConfigurator](../masstransit/iconsumeconfigurator), [IConsumerConfigurationObserverConnector](../masstransit/iconsumerconfigurationobserverconnector)

## Properties

### **ConcurrentMessageLimit**

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
