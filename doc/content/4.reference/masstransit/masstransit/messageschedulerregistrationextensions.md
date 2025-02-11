---

title: MessageSchedulerRegistrationExtensions

---

# MessageSchedulerRegistrationExtensions

Namespace: MassTransit

```csharp
public static class MessageSchedulerRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageSchedulerRegistrationExtensions](../masstransit/messageschedulerregistrationextensions)

## Methods

### **AddMessageScheduler(IBusRegistrationConfigurator, Uri)**

Add a  to the container that sends 
 to an external message scheduler on the specified endpoint address, such as Quartz or Hangfire.

```csharp
public static void AddMessageScheduler(IBusRegistrationConfigurator configurator, Uri schedulerEndpointAddress)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

`schedulerEndpointAddress` Uri<br/>
The endpoint address where the scheduler is running

### **AddMessageScheduler\<TBus\>(IBusRegistrationConfigurator\<TBus\>, Uri)**

Add a  to the container that sends 
 to an external message scheduler on the specified endpoint address, such as Quartz or Hangfire.

```csharp
public static void AddMessageScheduler<TBus>(IBusRegistrationConfigurator<TBus> configurator, Uri schedulerEndpointAddress)
```

#### Type Parameters

`TBus`<br/>

#### Parameters

`configurator` [IBusRegistrationConfigurator\<TBus\>](../masstransit/ibusregistrationconfigurator-1)<br/>

`schedulerEndpointAddress` Uri<br/>
The endpoint address where the scheduler is running

### **AddPublishMessageScheduler(IBusRegistrationConfigurator)**

Add a  to the container that publishes 
 to an external message scheduler, such as Quartz or Hangfire.

```csharp
public static void AddPublishMessageScheduler(IBusRegistrationConfigurator configurator)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

### **AddPublishMessageScheduler\<TBus\>(IBusRegistrationConfigurator\<TBus\>)**

Add a  to the container that publishes 
 to an external message scheduler, such as Quartz or Hangfire.

```csharp
public static void AddPublishMessageScheduler<TBus>(IBusRegistrationConfigurator<TBus> configurator)
```

#### Type Parameters

`TBus`<br/>

#### Parameters

`configurator` [IBusRegistrationConfigurator\<TBus\>](../masstransit/ibusregistrationconfigurator-1)<br/>
