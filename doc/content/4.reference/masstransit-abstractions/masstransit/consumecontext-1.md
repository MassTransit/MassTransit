---

title: ConsumeContext<T>

---

# ConsumeContext\<T\>

Namespace: MassTransit

```csharp
public interface ConsumeContext<T> : ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`T`<br/>

Implements [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Message**

```csharp
public abstract T Message { get; }
```

#### Property Value

T<br/>

## Methods

### **NotifyConsumed(TimeSpan, String)**

Notify that the message has been consumed -- note that this is internal, and should not be called by a consumer

```csharp
Task NotifyConsumed(TimeSpan duration, string consumerType)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The consumer type

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted(TimeSpan, String, Exception)**

Notify that a fault occurred during message consumption -- note that this is internal, and should not be called by a consumer

```csharp
Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
