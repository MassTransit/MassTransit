---

title: TimeoutConsumerConfigurationObserver<TConsumer>

---

# TimeoutConsumerConfigurationObserver\<TConsumer\>

Namespace: MassTransit.Configuration

```csharp
public class TimeoutConsumerConfigurationObserver<TConsumer> : IConsumerConfigurationObserver
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimeoutConsumerConfigurationObserver\<TConsumer\>](../masstransit-configuration/timeoutconsumerconfigurationobserver-1)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)

## Constructors

### **TimeoutConsumerConfigurationObserver(IConsumerConfigurator\<TConsumer\>, Action\<ITimeoutConfigurator\>)**

```csharp
public TimeoutConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, Action<ITimeoutConfigurator> configure)
```

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`configure` [Action\<ITimeoutConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
