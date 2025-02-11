---

title: JobServiceConfigurator<TReceiveEndpointConfigurator>

---

# JobServiceConfigurator\<TReceiveEndpointConfigurator\>

Namespace: MassTransit.Configuration

```csharp
public class JobServiceConfigurator<TReceiveEndpointConfigurator> : IJobServiceConfigurator, ISpecification
```

#### Type Parameters

`TReceiveEndpointConfigurator`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceConfigurator\<TReceiveEndpointConfigurator\>](../masstransit-configuration/jobserviceconfigurator-1)<br/>
Implements [IJobServiceConfigurator](../masstransit/ijobserviceconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Repository**

```csharp
public ISagaRepository<JobTypeSaga> Repository { set; }
```

#### Property Value

[ISagaRepository\<JobTypeSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

### **JobRepository**

```csharp
public ISagaRepository<JobSaga> JobRepository { set; }
```

#### Property Value

[ISagaRepository\<JobSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

### **JobAttemptRepository**

```csharp
public ISagaRepository<JobAttemptSaga> JobAttemptRepository { set; }
```

#### Property Value

[ISagaRepository\<JobAttemptSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

### **JobServiceStateEndpointName**

```csharp
public string JobServiceStateEndpointName { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JobServiceJobStateEndpointName**

```csharp
public string JobServiceJobStateEndpointName { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **JobServiceJobAttemptStateEndpointName**

```csharp
public string JobServiceJobAttemptStateEndpointName { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SlotWaitTime**

```csharp
public TimeSpan SlotWaitTime { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **StatusCheckInterval**

```csharp
public TimeSpan StatusCheckInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **SuspectJobRetryCount**

```csharp
public int SuspectJobRetryCount { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **SuspectJobRetryDelay**

```csharp
public TimeSpan SuspectJobRetryDelay { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **SagaPartitionCount**

```csharp
public Nullable<int> SagaPartitionCount { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **FinalizeCompleted**

```csharp
public bool FinalizeCompleted { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **JobServiceConfigurator(IServiceInstanceConfigurator\<TReceiveEndpointConfigurator\>, JobServiceOptions)**

```csharp
public JobServiceConfigurator(IServiceInstanceConfigurator<TReceiveEndpointConfigurator> instanceConfigurator, JobServiceOptions options)
```

#### Parameters

`instanceConfigurator` [IServiceInstanceConfigurator\<TReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>

`options` [JobServiceOptions](../masstransit/jobserviceoptions)<br/>

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **OnConfigureEndpoint(Action\<IReceiveEndpointConfigurator\>)**

```csharp
public void OnConfigureEndpoint(Action<IReceiveEndpointConfigurator> callback)
```

#### Parameters

`callback` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureJobServiceEndpoints(IRegistrationContext)**

```csharp
public void ConfigureJobServiceEndpoints(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
