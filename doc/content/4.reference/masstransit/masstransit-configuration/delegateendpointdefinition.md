---

title: DelegateEndpointDefinition

---

# DelegateEndpointDefinition

Namespace: MassTransit.Configuration

```csharp
public class DelegateEndpointDefinition : IEndpointDefinition
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegateEndpointDefinition](../masstransit-configuration/delegateendpointdefinition)<br/>
Implements [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)

## Properties

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

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

## Constructors

### **DelegateEndpointDefinition(String, IDefinition, IEndpointDefinition)**

```csharp
public DelegateEndpointDefinition(string endpointName, IDefinition definition, IEndpointDefinition endpointDefinition)
```

#### Parameters

`endpointName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`definition` [IDefinition](../../masstransit-abstractions/masstransit/idefinition)<br/>

`endpointDefinition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

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
