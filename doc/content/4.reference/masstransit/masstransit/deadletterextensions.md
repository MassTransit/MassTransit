---

title: DeadLetterExtensions

---

# DeadLetterExtensions

Namespace: MassTransit

```csharp
public static class DeadLetterExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DeadLetterExtensions](../masstransit/deadletterextensions)

## Methods

### **UseDeadLetter(IPipeConfigurator\<ReceiveContext\>, IPipe\<ReceiveContext\>)**

Rescue exceptions via the alternate pipe

```csharp
public static void UseDeadLetter(IPipeConfigurator<ReceiveContext> configurator, IPipe<ReceiveContext> rescuePipe)
```

#### Parameters

`configurator` [IPipeConfigurator\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`rescuePipe` [IPipe\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
