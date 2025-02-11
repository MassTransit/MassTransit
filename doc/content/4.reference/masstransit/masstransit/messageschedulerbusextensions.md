---

title: MessageSchedulerBusExtensions

---

# MessageSchedulerBusExtensions

Namespace: MassTransit

```csharp
public static class MessageSchedulerBusExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageSchedulerBusExtensions](../masstransit/messageschedulerbusextensions)

## Methods

### **CreateMessageScheduler(IBus, Uri)**

Create a message scheduler that uses an external message scheduler, such as Quartz.NET or Hangfire, to
 schedule messages. This should not be used with the broker-specific message schedulers.
 NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
 use the ScheduleSend extensions on ConsumeContext.

```csharp
public static IMessageScheduler CreateMessageScheduler(IBus bus, Uri schedulerEndpointAddress)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

`schedulerEndpointAddress` Uri<br/>
The endpoint address of the scheduler service

#### Returns

[IMessageScheduler](../../masstransit-abstractions/masstransit/imessagescheduler)<br/>

### **CreateMessageScheduler(ISendEndpointProvider, IBusTopology, Uri)**

Create a message scheduler that uses an external message scheduler, such as Quartz.NET or Hangfire, to
 schedule messages. This should not be used with the broker-specific message schedulers.
 NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
 use the ScheduleSend extensions on ConsumeContext.

```csharp
public static IMessageScheduler CreateMessageScheduler(ISendEndpointProvider sendEndpointProvider, IBusTopology busTopology, Uri schedulerEndpointAddress)
```

#### Parameters

`sendEndpointProvider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`busTopology` [IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

`schedulerEndpointAddress` Uri<br/>
The endpoint address of the scheduler service

#### Returns

[IMessageScheduler](../../masstransit-abstractions/masstransit/imessagescheduler)<br/>

### **CreateMessageScheduler(IBus)**

Create a message scheduler that uses an external message scheduler, such as Quartz.NET or Hangfire, to
 schedule messages. This should not be used with the broker-specific message schedulers. Scheduled messages
 are published to the external message scheduler, rather than uses a preconfigured endpoint address.
 NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
 use the ScheduleSend extensions on ConsumeContext.

```csharp
public static IMessageScheduler CreateMessageScheduler(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

#### Returns

[IMessageScheduler](../../masstransit-abstractions/masstransit/imessagescheduler)<br/>

### **CreateMessageScheduler(IPublishEndpoint, IBusTopology)**

Create a message scheduler that uses an external message scheduler, such as Quartz.NET or Hangfire, to
 schedule messages. This should not be used with the broker-specific message schedulers. Scheduled messages
 are published to the external message scheduler, rather than uses a preconfigured endpoint address.
 NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
 use the ScheduleSend extensions on ConsumeContext.

```csharp
public static IMessageScheduler CreateMessageScheduler(IPublishEndpoint publishEndpoint, IBusTopology busTopology)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`busTopology` [IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

#### Returns

[IMessageScheduler](../../masstransit-abstractions/masstransit/imessagescheduler)<br/>

### **CreateDelayedMessageScheduler(IBus)**

Create a message scheduler that uses the built-in transport message delay to schedule messages.
 NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
 use the ScheduleSend extensions on ConsumeContext.

```csharp
public static IMessageScheduler CreateDelayedMessageScheduler(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

#### Returns

[IMessageScheduler](../../masstransit-abstractions/masstransit/imessagescheduler)<br/>

### **CreateDelayedMessageScheduler(ISendEndpointProvider, IBusTopology)**

Create a message scheduler that uses the built-in transport message delay to schedule messages.
 NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
 use the ScheduleSend extensions on ConsumeContext.

```csharp
public static IMessageScheduler CreateDelayedMessageScheduler(ISendEndpointProvider sendEndpointProvider, IBusTopology busTopology)
```

#### Parameters

`sendEndpointProvider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`busTopology` [IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

#### Returns

[IMessageScheduler](../../masstransit-abstractions/masstransit/imessagescheduler)<br/>
