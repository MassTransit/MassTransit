---

title: JobSagaDefinition

---

# JobSagaDefinition

Namespace: MassTransit.Configuration

```csharp
public class JobSagaDefinition : SagaDefinition<JobSaga>, ISagaDefinition<JobSaga>, ISagaDefinition, IDefinition
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaDefinition\<JobSaga\>](../../masstransit-abstractions/masstransit/sagadefinition-1) → [JobSagaDefinition](../masstransit-configuration/jobsagadefinition)<br/>
Implements [ISagaDefinition\<JobSaga\>](../../masstransit-abstractions/masstransit/isagadefinition-1), [ISagaDefinition](../../masstransit-abstractions/masstransit/isagadefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition)

## Properties

### **EndpointDefinition**

```csharp
public IEndpointDefinition<JobSaga> EndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<JobSaga\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **JobSagaDefinition(IOptions\<JobSagaOptions\>)**

```csharp
public JobSagaDefinition(IOptions<JobSagaOptions> options)
```

#### Parameters

`options` IOptions\<JobSagaOptions\><br/>

## Methods

### **ConfigureSaga(IReceiveEndpointConfigurator, ISagaConfigurator\<JobSaga\>, IRegistrationContext)**

```csharp
protected void ConfigureSaga(IReceiveEndpointConfigurator configurator, ISagaConfigurator<JobSaga> sagaConfigurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`sagaConfigurator` [ISagaConfigurator\<JobSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
