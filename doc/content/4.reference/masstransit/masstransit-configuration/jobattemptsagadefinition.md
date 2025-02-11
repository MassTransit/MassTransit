---

title: JobAttemptSagaDefinition

---

# JobAttemptSagaDefinition

Namespace: MassTransit.Configuration

```csharp
public class JobAttemptSagaDefinition : SagaDefinition<JobAttemptSaga>, ISagaDefinition<JobAttemptSaga>, ISagaDefinition, IDefinition
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaDefinition\<JobAttemptSaga\>](../../masstransit-abstractions/masstransit/sagadefinition-1) → [JobAttemptSagaDefinition](../masstransit-configuration/jobattemptsagadefinition)<br/>
Implements [ISagaDefinition\<JobAttemptSaga\>](../../masstransit-abstractions/masstransit/isagadefinition-1), [ISagaDefinition](../../masstransit-abstractions/masstransit/isagadefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition)

## Properties

### **EndpointDefinition**

```csharp
public IEndpointDefinition<JobAttemptSaga> EndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<JobAttemptSaga\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **JobAttemptSagaDefinition(IOptions\<JobSagaOptions\>)**

```csharp
public JobAttemptSagaDefinition(IOptions<JobSagaOptions> options)
```

#### Parameters

`options` IOptions\<JobSagaOptions\><br/>

## Methods

### **ConfigureSaga(IReceiveEndpointConfigurator, ISagaConfigurator\<JobAttemptSaga\>, IRegistrationContext)**

```csharp
protected void ConfigureSaga(IReceiveEndpointConfigurator configurator, ISagaConfigurator<JobAttemptSaga> sagaConfigurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`sagaConfigurator` [ISagaConfigurator\<JobAttemptSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
