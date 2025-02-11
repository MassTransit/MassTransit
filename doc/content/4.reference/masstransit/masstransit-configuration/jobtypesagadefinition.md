---

title: JobTypeSagaDefinition

---

# JobTypeSagaDefinition

Namespace: MassTransit.Configuration

```csharp
public class JobTypeSagaDefinition : SagaDefinition<JobTypeSaga>, ISagaDefinition<JobTypeSaga>, ISagaDefinition, IDefinition
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaDefinition\<JobTypeSaga\>](../../masstransit-abstractions/masstransit/sagadefinition-1) → [JobTypeSagaDefinition](../masstransit-configuration/jobtypesagadefinition)<br/>
Implements [ISagaDefinition\<JobTypeSaga\>](../../masstransit-abstractions/masstransit/isagadefinition-1), [ISagaDefinition](../../masstransit-abstractions/masstransit/isagadefinition), [IDefinition](../../masstransit-abstractions/masstransit/idefinition)

## Properties

### **EndpointDefinition**

```csharp
public IEndpointDefinition<JobTypeSaga> EndpointDefinition { get; set; }
```

#### Property Value

[IEndpointDefinition\<JobTypeSaga\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; protected set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **JobTypeSagaDefinition(IOptions\<JobSagaOptions\>)**

```csharp
public JobTypeSagaDefinition(IOptions<JobSagaOptions> options)
```

#### Parameters

`options` IOptions\<JobSagaOptions\><br/>

## Methods

### **ConfigureSaga(IReceiveEndpointConfigurator, ISagaConfigurator\<JobTypeSaga\>, IRegistrationContext)**

```csharp
protected void ConfigureSaga(IReceiveEndpointConfigurator configurator, ISagaConfigurator<JobTypeSaga> sagaConfigurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`sagaConfigurator` [ISagaConfigurator\<JobTypeSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
