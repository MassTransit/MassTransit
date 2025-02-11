---

title: CourierContext

---

# CourierContext

Namespace: MassTransit

```csharp
public interface CourierContext : ConsumeContext<RoutingSlip>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

Implements [ConsumeContext\<RoutingSlip\>](../masstransit/consumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **TrackingNumber**

The tracking number for this routing slip

```csharp
public abstract Guid TrackingNumber { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ActivityName**

The name of the activity

```csharp
public abstract string ActivityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExecutionId**

The executionId for this attempt at the activity

```csharp
public abstract Guid ExecutionId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

The start time for the activity execution

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Elapsed**

The time elapsed for the execution operation

```csharp
public abstract TimeSpan Elapsed { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
