---

title: JobOptions<TJob>

---

# JobOptions\<TJob\>

Namespace: MassTransit

JobOptions contains the options used to configure the job consumer and related components

```csharp
public class JobOptions<TJob> : IOptions, ISpecification
```

#### Type Parameters

`TJob`<br/>
The Job Type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>
Implements [IOptions](../../masstransit-abstractions/masstransit-configuration/ioptions), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **JobTimeout**

Set the allowed time for a job to complete (per attempt). If the job timeout expires and the job has not yet completed, it will be canceled.

```csharp
public TimeSpan JobTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **JobCancellationTimeout**

Set the allowed time for a job to stop execution after the cancellation. If the job cancellation timeout expires and the job has not yet completed, it will be
 fully canceled.

```csharp
public TimeSpan JobCancellationTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **ConcurrentJobLimit**

Set the concurrent job limit. The limit is applied to each instance if the job consumer is scaled out.
 Do not use ConcurrentMessageLimit with job consumers."/&gt;

```csharp
public int ConcurrentJobLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **RetryPolicy**

```csharp
public IRetryPolicy RetryPolicy { get; private set; }
```

#### Property Value

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **JobTypeName**

Override the default job name (optional, automatically generated from the job type otherwise) that is displayed in the [JobTypeSaga](../masstransit/jobtypesaga).

```csharp
public string JobTypeName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ProgressBuffer**

Configure the job progress buffer settings, if using job progress (optional)

```csharp
public ProgressBufferSettings ProgressBuffer { get; }
```

#### Property Value

[ProgressBufferSettings](../masstransit/progressbuffersettings)<br/>

### **JobTypeProperties**

Properties that are specific to the job type, which can be used by the job distribution strategy

```csharp
public JobPropertyCollection JobTypeProperties { get; }
```

#### Property Value

[JobPropertyCollection](../masstransit-serialization/jobpropertycollection)<br/>

### **InstanceProperties**

Properties that are specific to this job consumer bus instance, such as region, data center, tenant, etc. also used by the job distribution strategy

```csharp
public JobPropertyCollection InstanceProperties { get; }
```

#### Property Value

[JobPropertyCollection](../masstransit-serialization/jobpropertycollection)<br/>

### **GlobalConcurrentJobLimit**

Optional, if specified, configures a global concurrent job limit across all job consumer instances

```csharp
public Nullable<int> GlobalConcurrentJobLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **JobOptions()**

```csharp
public JobOptions()
```

## Methods

### **SetJobTypeProperties(Action\<ISetPropertyCollection\>)**

Set job type properties that can be used by a custom job distribution strategy

```csharp
public JobOptions<TJob> SetJobTypeProperties(Action<ISetPropertyCollection> callback)
```

#### Parameters

`callback` [Action\<ISetPropertyCollection\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

### **SetInstanceProperties(Action\<ISetPropertyCollection\>)**

Set instance properties that can be used by a custom job distribution strategy

```csharp
public JobOptions<TJob> SetInstanceProperties(Action<ISetPropertyCollection> callback)
```

#### Parameters

`callback` [Action\<ISetPropertyCollection\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

### **SetJobTimeout(TimeSpan)**

Set the allowed time for a job to complete (per attempt). If the job timeout expires and the job has not yet completed, it will be canceled.

```csharp
public JobOptions<TJob> SetJobTimeout(TimeSpan timeout)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

### **SetJobCancellationTimeout(TimeSpan)**

Set the allowed time for a job to stop execution after the cancellation. If the job cancellation timeout expires and the job has not yet completed, it will be
 fully canceled.

```csharp
public JobOptions<TJob> SetJobCancellationTimeout(TimeSpan timeout)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

### **SetConcurrentJobLimit(Int32)**

Set the concurrent job limit. The limit is applied to each instance if the job consumer is scaled out.
 Do not use ConcurrentMessageLimit with job consumers."/&gt;

```csharp
public JobOptions<TJob> SetConcurrentJobLimit(int limit)
```

#### Parameters

`limit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

### **SetGlobalConcurrentJobLimit(Nullable\<Int32\>)**

Set the global concurrent job limit across all job consumer instances
 Do not use ConcurrentMessageLimit with job consumers."/&gt;

```csharp
public JobOptions<TJob> SetGlobalConcurrentJobLimit(Nullable<int> limit)
```

#### Parameters

`limit` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

### **SetJobTypeName(String)**

Override the default job name (optional, automatically generated from the job type otherwise) that is displayed in the [JobTypeSaga](../masstransit/jobtypesaga).

```csharp
public JobOptions<TJob> SetJobTypeName(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

### **SetRetry(Action\<IRetryConfigurator\>)**

Set the job retry policy, used to handle faulted jobs. Retry middleware on the job consumer endpoint is not used.

```csharp
public JobOptions<TJob> SetRetry(Action<IRetryConfigurator> configure)
```

#### Parameters

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

### **SetProgressBuffer(Nullable\<Int32\>, Nullable\<TimeSpan\>)**

Set the job progress buffer settings, either value can be set and will update the settings

```csharp
public JobOptions<TJob> SetProgressBuffer(Nullable<int> updateLimit, Nullable<TimeSpan> timeLimit)
```

#### Parameters

`updateLimit` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
The number of updates to buffer before sending the most recent update to the job saga

`timeLimit` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
The time since the first update after the last update sent to the job saga before an update must be sent

#### Returns

[JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>
