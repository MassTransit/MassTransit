---

title: JobService

---

# JobService

Namespace: MassTransit.JobService

```csharp
public class JobService : IJobService
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobService](../masstransit-jobservice/jobservice)<br/>
Implements [IJobService](../masstransit-jobservice/ijobservice)

## Properties

### **Settings**

```csharp
public JobServiceSettings Settings { get; }
```

#### Property Value

[JobServiceSettings](../masstransit-jobservice/jobservicesettings)<br/>

### **InstanceAddress**

```csharp
public Uri InstanceAddress { get; }
```

#### Property Value

Uri<br/>

## Constructors

### **JobService(JobServiceSettings)**

```csharp
public JobService(JobServiceSettings settings)
```

#### Parameters

`settings` [JobServiceSettings](../masstransit-jobservice/jobservicesettings)<br/>

## Methods

### **TryGetJob(Guid, JobHandle)**

```csharp
public bool TryGetJob(Guid jobId, out JobHandle jobReference)
```

#### Parameters

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`jobReference` [JobHandle](../masstransit-jobservice/jobhandle)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryRemoveJob(Guid, JobHandle)**

```csharp
public bool TryRemoveJob(Guid jobId, out JobHandle jobHandle)
```

#### Parameters

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`jobHandle` [JobHandle](../masstransit-jobservice/jobhandle)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **StartJob\<T\>(ConsumeContext\<StartJob\>, T, IPipe\<ConsumeContext\<T\>\>, JobOptions\<T\>)**

```csharp
public Task<JobHandle> StartJob<T>(ConsumeContext<StartJob> context, T job, IPipe<ConsumeContext<T>> jobPipe, JobOptions<T> jobOptions)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<StartJob\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`job` T<br/>

`jobPipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`jobOptions` [JobOptions\<T\>](../masstransit/joboptions-1)<br/>

#### Returns

[Task\<JobHandle\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Stop(IPublishEndpoint)**

```csharp
public Task Stop(IPublishEndpoint publishEndpoint)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RegisterJobType\<T\>(IReceiveEndpointConfigurator, JobOptions\<T\>, Guid, String)**

```csharp
public void RegisterJobType<T>(IReceiveEndpointConfigurator configurator, JobOptions<T> options, Guid jobTypeId, string jobTypeName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`options` [JobOptions\<T\>](../masstransit/joboptions-1)<br/>

`jobTypeId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`jobTypeName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **BusStarted(IPublishEndpoint)**

```csharp
public Task BusStarted(IPublishEndpoint publishEndpoint)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **GetJobTypeId\<T\>()**

```csharp
public Guid GetJobTypeId<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ConfigureSuperviseJobConsumer(IReceiveEndpointConfigurator)**

```csharp
public void ConfigureSuperviseJobConsumer(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
