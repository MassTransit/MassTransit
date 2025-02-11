---

title: MessageSchedulerExtensions

---

# MessageSchedulerExtensions

Namespace: MassTransit

```csharp
public static class MessageSchedulerExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageSchedulerExtensions](../masstransit/messageschedulerextensions)

## Methods

### **UseMessageScheduler(IConsumePipeConfigurator, Uri)**

Specify an endpoint to use for message scheduling

```csharp
public static void UseMessageScheduler(IConsumePipeConfigurator configurator, Uri schedulerAddress)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`schedulerAddress` Uri<br/>

### **UsePublishMessageScheduler(IConsumePipeConfigurator)**

Uses Publish (instead of Send) to schedule messages via the Quartz message scheduler. For this to work, a single
 queue should be used to schedule all messages. If multiple instances are running, they should be on the same Quartz
 cluster.

```csharp
public static void UsePublishMessageScheduler(IConsumePipeConfigurator configurator)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>
