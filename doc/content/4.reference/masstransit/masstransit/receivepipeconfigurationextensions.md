---

title: ReceivePipeConfigurationExtensions

---

# ReceivePipeConfigurationExtensions

Namespace: MassTransit

```csharp
public static class ReceivePipeConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceivePipeConfigurationExtensions](../masstransit/receivepipeconfigurationextensions)

## Methods

### **ConfigureDefaultDeadLetterTransport(IReceivePipelineConfigurator)**

Use the default _skipped transport for messages that are not consumed

```csharp
public static void ConfigureDefaultDeadLetterTransport(IReceivePipelineConfigurator configurator)
```

#### Parameters

`configurator` [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator)<br/>

### **DiscardSkippedMessages(IReceivePipelineConfigurator)**

Messages that are not consumed should be discarded instead of being moved to _skipped queue

```csharp
public static void DiscardSkippedMessages(IReceivePipelineConfigurator configurator)
```

#### Parameters

`configurator` [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator)<br/>

### **ConfigureDefaultErrorTransport(IReceivePipelineConfigurator)**

Generate a  event and move the message to the _error transport.

```csharp
public static void ConfigureDefaultErrorTransport(IReceivePipelineConfigurator configurator)
```

#### Parameters

`configurator` [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator)<br/>

### **DiscardFaultedMessages(IReceivePipelineConfigurator)**

Messages that fault should be discarded instead of being moved to the _error queue. Fault events
 will still be published.

```csharp
public static void DiscardFaultedMessages(IReceivePipelineConfigurator configurator)
```

#### Parameters

`configurator` [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator)<br/>

### **RethrowFaultedMessages(IReceivePipelineConfigurator)**

Messages that fault should throw exceptions, suppressing the default error queue behavior

```csharp
public static void RethrowFaultedMessages(IReceivePipelineConfigurator configurator)
```

#### Parameters

`configurator` [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator)<br/>

### **ThrowOnSkippedMessages(IReceivePipelineConfigurator)**

Messages that are not consumed should throw an exception, forcing the default dead letter behavior

```csharp
public static void ThrowOnSkippedMessages(IReceivePipelineConfigurator configurator)
```

#### Parameters

`configurator` [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator)<br/>
