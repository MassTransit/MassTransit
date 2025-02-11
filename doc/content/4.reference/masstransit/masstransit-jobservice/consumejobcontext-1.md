---

title: ConsumeJobContext<TJob>

---

# ConsumeJobContext\<TJob\>

Namespace: MassTransit.JobService

```csharp
public class ConsumeJobContext<TJob> : ConsumeContextProxy, IPublishEndpoint, IPublishObserverConnector, ConsumeContext, PipeContext, MessageContext, ISendEndpointProvider, ISendObserverConnector, ConsumeContext<TJob>, JobContext<TJob>, JobContext, INotifyJobContext, IAsyncDisposable
```

#### Type Parameters

`TJob`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpoint](../../masstransit-abstractions/masstransit-transports/publishendpoint) → [BaseConsumeContext](../masstransit-context/baseconsumecontext) → [ConsumeContextProxy](../masstransit-context/consumecontextproxy) → [ConsumeJobContext\<TJob\>](../masstransit-jobservice/consumejobcontext-1)<br/>
Implements [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ConsumeContext\<TJob\>](../../masstransit-abstractions/masstransit/consumecontext-1), [JobContext\<TJob\>](../../masstransit-abstractions/masstransit/jobcontext-1), [JobContext](../../masstransit-abstractions/masstransit/jobcontext), [INotifyJobContext](../../masstransit-abstractions/masstransit/inotifyjobcontext), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Message**

```csharp
public TJob Message { get; }
```

#### Property Value

TJob<br/>

### **JobId**

```csharp
public Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

```csharp
public Guid AttemptId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **RetryAttempt**

```csharp
public int RetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **LastProgressValue**

```csharp
public Nullable<long> LastProgressValue { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LastProgressLimit**

```csharp
public Nullable<long> LastProgressLimit { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Job**

```csharp
public TJob Job { get; }
```

#### Property Value

TJob<br/>

### **ElapsedTime**

```csharp
public TimeSpan ElapsedTime { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **JobProperties**

```csharp
public IPropertyCollection JobProperties { get; set; }
```

#### Property Value

[IPropertyCollection](../../masstransit-abstractions/masstransit/ipropertycollection)<br/>

### **JobTypeProperties**

```csharp
public IPropertyCollection JobTypeProperties { get; }
```

#### Property Value

[IPropertyCollection](../../masstransit-abstractions/masstransit/ipropertycollection)<br/>

### **InstanceProperties**

```csharp
public IPropertyCollection InstanceProperties { get; }
```

#### Property Value

[IPropertyCollection](../../masstransit-abstractions/masstransit/ipropertycollection)<br/>

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

```csharp
public Uri SourceAddress { get; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public Uri DestinationAddress { get; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public Headers Headers { get; }
```

#### Property Value

[Headers](../../masstransit-abstractions/masstransit/headers)<br/>

### **Host**

```csharp
public HostInfo Host { get; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

### **ConsumeCompleted**

```csharp
public Task ConsumeCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SupportedMessageTypes**

```csharp
public IEnumerable<string> SupportedMessageTypes { get; }
```

#### Property Value

[IEnumerable\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **ReceiveContext**

```csharp
public ReceiveContext ReceiveContext { get; protected set; }
```

#### Property Value

[ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

### **SerializerContext**

```csharp
public SerializerContext SerializerContext { get; }
```

#### Property Value

[SerializerContext](../../masstransit-abstractions/masstransit/serializercontext)<br/>

## Constructors

### **ConsumeJobContext(ConsumeContext\<StartJob\>, Uri, TJob, JobOptions\<TJob\>)**

```csharp
public ConsumeJobContext(ConsumeContext<StartJob> context, Uri instanceAddress, TJob job, JobOptions<TJob> jobOptions)
```

#### Parameters

`context` [ConsumeContext\<StartJob\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`instanceAddress` Uri<br/>

`job` TJob<br/>

`jobOptions` [JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

## Methods

### **NotifyConsumed(TimeSpan, String)**

```csharp
public Task NotifyConsumed(TimeSpan duration, string consumerType)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted(TimeSpan, String, Exception)**

```csharp
public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **NotifyCanceled()**

```csharp
public Task NotifyCanceled()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyStarted()**

```csharp
public Task NotifyStarted()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyCompleted()**

```csharp
public Task NotifyCompleted()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyJobProgress(SetJobProgress)**

```csharp
public Task NotifyJobProgress(SetJobProgress progress)
```

#### Parameters

`progress` [SetJobProgress](../../masstransit-abstractions/masstransit-contracts-jobservice/setjobprogress)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted(Exception, Nullable\<TimeSpan\>)**

```csharp
public Task NotifyFaulted(Exception exception, Nullable<TimeSpan> delay)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`delay` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SetJobProgress(Int64, Nullable\<Int64\>)**

```csharp
public Task SetJobProgress(long value, Nullable<long> limit)
```

#### Parameters

`value` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`limit` [Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SaveJobState\<T\>(T)**

```csharp
public Task SaveJobState<T>(T jobState)
```

#### Type Parameters

`T`<br/>

#### Parameters

`jobState` T<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **TryGetJobState\<T\>(T)**

```csharp
public bool TryGetJobState<T>(out T jobState)
```

#### Type Parameters

`T`<br/>

#### Parameters

`jobState` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Cancel(String)**

```csharp
public void Cancel(string reason)
```

#### Parameters

`reason` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
