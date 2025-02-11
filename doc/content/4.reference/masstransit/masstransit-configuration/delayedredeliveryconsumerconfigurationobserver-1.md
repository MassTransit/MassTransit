---

title: DelayedRedeliveryConsumerConfigurationObserver<TConsumer>

---

# DelayedRedeliveryConsumerConfigurationObserver\<TConsumer\>

Namespace: MassTransit.Configuration

Configures scheduled message redelivery for a consumer, on the consumer configurator, which is constrained to
 the message types for that consumer, and only applies to the consumer prior to the consumer factory.

```csharp
public class DelayedRedeliveryConsumerConfigurationObserver<TConsumer> : IConsumerConfigurationObserver
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelayedRedeliveryConsumerConfigurationObserver\<TConsumer\>](../masstransit-configuration/delayedredeliveryconsumerconfigurationobserver-1)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)

## Constructors

### **DelayedRedeliveryConsumerConfigurationObserver(IConsumerConfigurator\<TConsumer\>, Action\<IRedeliveryConfigurator\>)**

```csharp
public DelayedRedeliveryConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, Action<IRedeliveryConfigurator> configure)
```

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`configure` [Action\<IRedeliveryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
