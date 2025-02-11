---

title: JobServiceExtensions

---

# JobServiceExtensions

Namespace: MassTransit

```csharp
public static class JobServiceExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceExtensions](../masstransit/jobserviceextensions)

## Methods

### **GetJobState(IRequestClient\<GetJobState\>, Guid)**

Requests the job state for the specified  using the request client

```csharp
public static Task<JobState> GetJobState(IRequestClient<GetJobState> client, Guid jobId)
```

#### Parameters

`client` [IRequestClient\<GetJobState\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<JobState\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetJobState\<T\>(IRequestClient\<GetJobState\>, Guid)**

Requests the job state for the specified  using the request client

```csharp
public static Task<JobState<T>> GetJobState<T>(IRequestClient<GetJobState> client, Guid jobId)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<GetJobState\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<JobState\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IPublishEndpoint, T, CancellationToken)**

Submits a job, returning the generated jobId

```csharp
public static Task<Guid> SubmitJob<T>(IPublishEndpoint publishEndpoint, T job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`job` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IPublishEndpoint, T, Action\<ISetPropertyCollection\>, CancellationToken)**

Submits a job, returning the generated jobId

```csharp
public static Task<Guid> SubmitJob<T>(IPublishEndpoint publishEndpoint, T job, Action<ISetPropertyCollection> setJobProperties, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`job` T<br/>

`setJobProperties` [Action\<ISetPropertyCollection\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IPublishEndpoint, Guid, T, Action\<ISetPropertyCollection\>, CancellationToken)**

Submits a job, returning the generated jobId

```csharp
public static Task<Guid> SubmitJob<T>(IPublishEndpoint publishEndpoint, Guid jobId, T job, Action<ISetPropertyCollection> setJobProperties, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
A unique job id

`job` T<br/>

`setJobProperties` [Action\<ISetPropertyCollection\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IPublishEndpoint, Object, CancellationToken)**

Submits a job, returning the generated jobId

```csharp
public static Task<Guid> SubmitJob<T>(IPublishEndpoint publishEndpoint, object job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`job` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IPublishEndpoint, Object, Action\<ISetPropertyCollection\>, CancellationToken)**

Submits a job, returning the generated jobId

```csharp
public static Task<Guid> SubmitJob<T>(IPublishEndpoint publishEndpoint, object job, Action<ISetPropertyCollection> setJobProperties, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`job` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`setJobProperties` [Action\<ISetPropertyCollection\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IPublishEndpoint, Guid, Object, Action\<ISetPropertyCollection\>, CancellationToken)**

Submits a job, returning the generated jobId

```csharp
public static Task<Guid> SubmitJob<T>(IPublishEndpoint publishEndpoint, Guid jobId, object job, Action<ISetPropertyCollection> setJobProperties, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
A unique job id

`job` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`setJobProperties` [Action\<ISetPropertyCollection\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, T, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> SubmitJob<T>(IRequestClient<SubmitJob<T>> client, T job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`job` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, T, Action\<ISetPropertyCollection\>, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> SubmitJob<T>(IRequestClient<SubmitJob<T>> client, T job, Action<ISetPropertyCollection> setJobProperties, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`job` T<br/>

`setJobProperties` [Action\<ISetPropertyCollection\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, Guid, T, Action\<ISetPropertyCollection\>, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> SubmitJob<T>(IRequestClient<SubmitJob<T>> client, Guid jobId, T job, Action<ISetPropertyCollection> setJobProperties, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
A unique job id

`job` T<br/>

`setJobProperties` [Action\<ISetPropertyCollection\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, Object, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> SubmitJob<T>(IRequestClient<SubmitJob<T>> client, object job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`job` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, Object, Action\<ISetPropertyCollection\>, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> SubmitJob<T>(IRequestClient<SubmitJob<T>> client, object job, Action<ISetPropertyCollection> setJobProperties, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`job` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`setJobProperties` [Action\<ISetPropertyCollection\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, Guid, Object, Action\<ISetPropertyCollection\>, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> SubmitJob<T>(IRequestClient<SubmitJob<T>> client, Guid jobId, object job, Action<ISetPropertyCollection> setJobProperties, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
Specify an explicit jobId for the job

`job` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`setJobProperties` [Action\<ISetPropertyCollection\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IRequestClient\<T\>, T, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> SubmitJob<T>(IRequestClient<T> client, T job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`job` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SubmitJob\<T\>(IRequestClient\<T\>, Object, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> SubmitJob<T>(IRequestClient<T> client, object job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`job` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CancelJob(IPublishEndpoint, Guid, String)**

Cancel a job if the job exists and is in a state that can be canceled.

```csharp
public static Task CancelJob(IPublishEndpoint publishEndpoint, Guid jobId, string reason)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`reason` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RetryJob(IPublishEndpoint, Guid)**

Retry a job if the job exists and is in a state that can be retried.

```csharp
public static Task RetryJob(IPublishEndpoint publishEndpoint, Guid jobId)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **FinalizeJob(IPublishEndpoint, Guid)**

Finalize a job, removing any faulted job attempts, so that it can be removed from the saga repository

```csharp
public static Task FinalizeJob(IPublishEndpoint publishEndpoint, Guid jobId)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
