---

title: SqlScheduleMessageProvider

---

# SqlScheduleMessageProvider

Namespace: MassTransit.Scheduling

```csharp
public class SqlScheduleMessageProvider : IScheduleMessageProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlScheduleMessageProvider](../masstransit-scheduling/sqlschedulemessageprovider)<br/>
Implements [IScheduleMessageProvider](../../masstransit-abstractions/masstransit/ischedulemessageprovider)

## Constructors

### **SqlScheduleMessageProvider(ConsumeContext)**

```csharp
public SqlScheduleMessageProvider(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

### **SqlScheduleMessageProvider(ISqlHostConfiguration, ISendEndpointProvider)**

```csharp
public SqlScheduleMessageProvider(ISqlHostConfiguration hostConfiguration, ISendEndpointProvider sendEndpointProvider)
```

#### Parameters

`hostConfiguration` [ISqlHostConfiguration](../masstransit-sqltransport-configuration/isqlhostconfiguration)<br/>

`sendEndpointProvider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

## Methods

### **ScheduleSend\<T\>(Uri, DateTime, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<ScheduledMessage\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CancelScheduledSend(Guid, CancellationToken)**

```csharp
public Task CancelScheduledSend(Guid tokenId, CancellationToken cancellationToken)
```

#### Parameters

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancelScheduledSend(Uri, Guid, CancellationToken)**

```csharp
public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId, CancellationToken cancellationToken)
```

#### Parameters

`destinationAddress` Uri<br/>

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
