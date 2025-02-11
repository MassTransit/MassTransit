---

title: JobContext

---

# JobContext

Namespace: MassTransit

```csharp
public interface JobContext : PipeContext, MessageContext, ISendEndpointProvider, ISendObserverConnector, IPublishEndpoint, IPublishObserverConnector
```

Implements [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector)

## Properties

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

```csharp
public abstract Guid AttemptId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **RetryAttempt**

If previously attempted, this value is &gt; 0

```csharp
public abstract int RetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **LastProgressValue**

The last reported progress value for this job

```csharp
public abstract Nullable<long> LastProgressValue { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LastProgressLimit**

The last reported progress limit for this job

```csharp
public abstract Nullable<long> LastProgressLimit { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ElapsedTime**

How long the job has been running

```csharp
public abstract TimeSpan ElapsedTime { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **JobProperties**

The job properties that were supplied when the job was submitted

```csharp
public abstract IPropertyCollection JobProperties { get; }
```

#### Property Value

[IPropertyCollection](../masstransit/ipropertycollection)<br/>

### **JobTypeProperties**

Properties that were configured for this job type

```csharp
public abstract IPropertyCollection JobTypeProperties { get; }
```

#### Property Value

[IPropertyCollection](../masstransit/ipropertycollection)<br/>

### **InstanceProperties**

Properties that were configured for this job consumer instance

```csharp
public abstract IPropertyCollection InstanceProperties { get; }
```

#### Property Value

[IPropertyCollection](../masstransit/ipropertycollection)<br/>

## Methods

### **SetJobProgress(Int64, Nullable\<Int64\>)**

Sets the job's progress, which gets reported back to the job saga

```csharp
Task SetJobProgress(long value, Nullable<long> limit)
```

#### Parameters

`value` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`limit` [Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SaveJobState\<T\>(T)**

Save job state, typically when canceling or faulting, so that subsequent retries can resume from the saved state

```csharp
Task SaveJobState<T>(T jobState)
```

#### Type Parameters

`T`<br/>

#### Parameters

`jobState` T<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **TryGetJobState\<T\>(T)**

```csharp
bool TryGetJobState<T>(out T jobState)
```

#### Type Parameters

`T`<br/>

#### Parameters

`jobState` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
