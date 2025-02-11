---

title: InstanceEndpointDefinition

---

# InstanceEndpointDefinition

Namespace: MassTransit.Configuration

Instance-specific address for a service endpoint

```csharp
public class InstanceEndpointDefinition : IEndpointDefinition
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstanceEndpointDefinition](../masstransit-configuration/instanceendpointdefinition)<br/>
Implements [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)

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

### **InstanceEndpointDefinition()**

```csharp
public InstanceEndpointDefinition()
```

## Methods

### **GetEndpointName(IEndpointNameFormatter)**

```csharp
public string GetEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Configure\<T\>(T, IRegistrationContext)**

```csharp
public void Configure<T>(T configurator, IRegistrationContext context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` T<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
