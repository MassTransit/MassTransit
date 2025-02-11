---

title: IJobService

---

# IJobService

Namespace: MassTransit.JobService

```csharp
public interface IJobService
```

## Properties

### **InstanceAddress**

```csharp
public abstract Uri InstanceAddress { get; }
```

#### Property Value

Uri<br/>

### **Settings**

```csharp
public abstract JobServiceSettings Settings { get; }
```

#### Property Value

[JobServiceSettings](../masstransit-jobservice/jobservicesettings)<br/>

## Methods

### **StartJob\<T\>(ConsumeContext\<StartJob\>, T, IPipe\<ConsumeContext\<T\>\>, JobOptions\<T\>)**

Starts a job

```csharp
Task<JobHandle> StartJob<T>(ConsumeContext<StartJob> context, T job, IPipe<ConsumeContext<T>> jobPipe, JobOptions<T> jobOptions)
```

#### Type Parameters

`T`<br/>
The message type that is used to initiate the job

#### Parameters

`context` [ConsumeContext\<StartJob\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>
The context of the message being consumed

`job` T<br/>
The job command

`jobPipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
The pipe which executes the job

`jobOptions` [JobOptions\<T\>](../masstransit/joboptions-1)<br/>
The job options

#### Returns

[Task\<JobHandle\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The newly created job's handle

### **Stop(IPublishEndpoint)**

Shut town the job service, cancelling any pending jobs

```csharp
Task Stop(IPublishEndpoint publishEndpoint)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **TryGetJob(Guid, JobHandle)**

```csharp
bool TryGetJob(Guid jobId, out JobHandle jobReference)
```

#### Parameters

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`jobReference` [JobHandle](../masstransit-jobservice/jobhandle)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryRemoveJob(Guid, JobHandle)**

Remove the job from the roster

```csharp
bool TryRemoveJob(Guid jobId, out JobHandle jobHandle)
```

#### Parameters

`jobId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`jobHandle` [JobHandle](../masstransit-jobservice/jobhandle)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **RegisterJobType\<T\>(IReceiveEndpointConfigurator, JobOptions\<T\>, Guid, String)**

Registers a job type at bus configuration time so that the options can be announced when the bus is started/stopped

```csharp
void RegisterJobType<T>(IReceiveEndpointConfigurator configurator, JobOptions<T> options, Guid jobTypeId, string jobTypeName)
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
Task BusStarted(IPublishEndpoint publishEndpoint)
```

#### Parameters

`publishEndpoint` [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **GetJobTypeId\<T\>()**

Return the registered JobTypeId for the job type

```csharp
Guid GetJobTypeId<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ConfigureSuperviseJobConsumer(IReceiveEndpointConfigurator)**

```csharp
void ConfigureSuperviseJobConsumer(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
