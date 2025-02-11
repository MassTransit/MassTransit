---

title: ScheduledRedeliveryConsumerConfigurationObserver<TConsumer>

---

# ScheduledRedeliveryConsumerConfigurationObserver\<TConsumer\>

Namespace: MassTransit.Configuration

Configures scheduled message redelivery for a consumer, on the consumer configurator, which is constrained to
 the message types for that consumer, and only applies to the consumer prior to the consumer factory.

```csharp
public class ScheduledRedeliveryConsumerConfigurationObserver<TConsumer> : IConsumerConfigurationObserver
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduledRedeliveryConsumerConfigurationObserver\<TConsumer\>](../masstransit-configuration/scheduledredeliveryconsumerconfigurationobserver-1)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)

## Constructors

### **ScheduledRedeliveryConsumerConfigurationObserver(IConsumerConfigurator\<TConsumer\>, Action\<IRetryConfigurator\>)**

```csharp
public ScheduledRedeliveryConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, Action<IRetryConfigurator> configure)
```

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
