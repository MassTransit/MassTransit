---

title: DelayedMessageSchedulerRegistrationExtensions

---

# DelayedMessageSchedulerRegistrationExtensions

Namespace: MassTransit

```csharp
public static class DelayedMessageSchedulerRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelayedMessageSchedulerRegistrationExtensions](../masstransit/delayedmessageschedulerregistrationextensions)

## Methods

### **AddDelayedMessageScheduler(IBusRegistrationConfigurator)**

Add a  to the container that uses transport message delay to schedule messages

```csharp
public static void AddDelayedMessageScheduler(IBusRegistrationConfigurator configurator)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

### **AddDelayedMessageScheduler\<TBus\>(IBusRegistrationConfigurator\<TBus\>)**

Add a  to the container that uses transport message delay to schedule messages

```csharp
public static void AddDelayedMessageScheduler<TBus>(IBusRegistrationConfigurator<TBus> configurator)
```

#### Type Parameters

`TBus`<br/>

#### Parameters

`configurator` [IBusRegistrationConfigurator\<TBus\>](../masstransit/ibusregistrationconfigurator-1)<br/>
