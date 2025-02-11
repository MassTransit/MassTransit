---

title: DelayedMessageSchedulerConfigurationExtensions

---

# DelayedMessageSchedulerConfigurationExtensions

Namespace: MassTransit

```csharp
public static class DelayedMessageSchedulerConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelayedMessageSchedulerConfigurationExtensions](../masstransit/delayedmessageschedulerconfigurationextensions)

## Methods

### **UseDelayedMessageScheduler(IBusFactoryConfigurator)**

Use the built-in transport message delay to schedule messages

```csharp
public static void UseDelayedMessageScheduler(IBusFactoryConfigurator configurator)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>
