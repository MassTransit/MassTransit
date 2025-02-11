---

title: RecurringJobConsumerExtensions

---

# RecurringJobConsumerExtensions

Namespace: MassTransit

```csharp
public static class RecurringJobConsumerExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RecurringJobConsumerExtensions](../masstransit/recurringjobconsumerextensions)

## Methods

### **AddOrUpdateRecurringJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, String, T, String, CancellationToken)**

Add or update a recurring job

```csharp
public static Task<Guid> AddOrUpdateRecurringJob<T>(IRequestClient<SubmitJob<T>> client, string jobName, T job, string cronExpression, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>
An existing request client

`jobName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`job` T<br/>

`cronExpression` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduler cron expression

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **AddOrUpdateRecurringJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, String, T, Action\<IRecurringJobScheduleConfigurator\>, CancellationToken)**

Add or update a recurring job

```csharp
public static Task<Guid> AddOrUpdateRecurringJob<T>(IRequestClient<SubmitJob<T>> client, string jobName, T job, Action<IRecurringJobScheduleConfigurator> configure, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>
An existing request client

`jobName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`job` T<br/>

`configure` [Action\<IRecurringJobScheduleConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the optional recurring job schedule parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **AddOrUpdateRecurringJob\<T\>(IPublishEndpoint, String, T, String, CancellationToken)**

Add or update a recurring job

```csharp
public static Task<Guid> AddOrUpdateRecurringJob<T>(IPublishEndpoint publishEndpoint, string jobName, T job, string cronExpression, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
An available publish endpoint instance

`jobName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`job` T<br/>

`cronExpression` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The scheduler cron expression

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **AddOrUpdateRecurringJob\<T\>(IPublishEndpoint, String, T, Action\<IRecurringJobScheduleConfigurator\>, CancellationToken)**

Add or update a recurring job

```csharp
public static Task<Guid> AddOrUpdateRecurringJob<T>(IPublishEndpoint publishEndpoint, string jobName, T job, Action<IRecurringJobScheduleConfigurator> configure, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
An available publish endpoint instance

`jobName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`job` T<br/>

`configure` [Action\<IRecurringJobScheduleConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the optional recurring job schedule parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CancelRecurringJob\<T\>(IPublishEndpoint, String, String, CancellationToken)**

Cancel a recurring job

```csharp
public static Task<Guid> CancelRecurringJob<T>(IPublishEndpoint publishEndpoint, string jobName, string reason, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
An available publish endpoint instance

`jobName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`reason` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The reason for canceling the job

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **FinalizeRecurringJob\<T\>(IPublishEndpoint, String, CancellationToken)**

Finalize a canceled recurring job

```csharp
public static Task<Guid> FinalizeRecurringJob<T>(IPublishEndpoint publishEndpoint, string jobName, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>
An available publish endpoint instance

`jobName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleJob\<T\>(IPublishEndpoint, DateTimeOffset, T, CancellationToken)**

Submits a job, returning the generated jobId

```csharp
public static Task<Guid> ScheduleJob<T>(IPublishEndpoint publishEndpoint, DateTimeOffset start, T job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`start` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>
The start time for the job

`job` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, DateTimeOffset, T, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> ScheduleJob<T>(IRequestClient<SubmitJob<T>> client, DateTimeOffset start, T job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`start` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>
The start time for the job

`job` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, DateTimeOffset, Object, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> ScheduleJob<T>(IRequestClient<SubmitJob<T>> client, DateTimeOffset start, object job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`start` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>
The start time for the job

`job` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, Guid, DateTimeOffset, T, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> ScheduleJob<T>(IRequestClient<SubmitJob<T>> client, Guid jobId, DateTimeOffset start, T job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
Specify an explicit jobId for the job

`start` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>
The start time for the job

`job` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ScheduleJob\<T\>(IRequestClient\<SubmitJob\<T\>\>, Guid, DateTimeOffset, Object, CancellationToken)**

Submits a job, returning the accepted jobId

```csharp
public static Task<Guid> ScheduleJob<T>(IRequestClient<SubmitJob<T>> client, Guid jobId, DateTimeOffset start, object job, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`client` [IRequestClient\<SubmitJob\<T\>\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
Specify an explicit jobId for the job

`start` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>
The start time for the job

`job` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **RunRecurringJob\<T\>(IPublishEndpoint, String)**

Run a recurring job if it's currently waiting/scheduled to run

```csharp
public static Task RunRecurringJob<T>(IPublishEndpoint publishEndpoint, string jobName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

`jobName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
