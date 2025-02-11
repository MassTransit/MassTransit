---

title: JobServiceEndpointDefinition

---

# JobServiceEndpointDefinition

Namespace: MassTransit.Configuration

```csharp
public class JobServiceEndpointDefinition : IEndpointDefinition<JobService>, IEndpointDefinition
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceEndpointDefinition](../masstransit-configuration/jobserviceendpointdefinition)<br/>
Implements [IEndpointDefinition\<JobService\>](../../masstransit-abstractions/masstransit/iendpointdefinition-1), [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)

## Properties

### **IsTemporary**

```csharp
public bool IsTemporary { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

```csharp
public Nullable<int> PrefetchCount { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **JobServiceEndpointDefinition(IEndpointSettings\<IEndpointDefinition\<JobService\>\>, InstanceJobServiceSettings)**

```csharp
public JobServiceEndpointDefinition(IEndpointSettings<IEndpointDefinition<JobService>> settings, InstanceJobServiceSettings jobServiceSettings)
```

#### Parameters

`settings` [IEndpointSettings\<IEndpointDefinition\<JobService\>\>](../../masstransit-abstractions/masstransit/iendpointsettings-1)<br/>

`jobServiceSettings` [InstanceJobServiceSettings](../masstransit-configuration/instancejobservicesettings)<br/>

## Methods

### **Configure\<T\>(T, IRegistrationContext)**

```csharp
public void Configure<T>(T configurator, IRegistrationContext context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` T<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **GetEndpointName(IEndpointNameFormatter)**

```csharp
public string GetEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
