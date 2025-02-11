---

title: TemporaryEndpointDefinition

---

# TemporaryEndpointDefinition

Namespace: MassTransit

Specifies a temporary endpoint, with the prefix "response"

```csharp
public class TemporaryEndpointDefinition : IEndpointDefinition
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TemporaryEndpointDefinition](../masstransit/temporaryendpointdefinition)<br/>
Implements [IEndpointDefinition](../masstransit/iendpointdefinition)

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

### **TemporaryEndpointDefinition(String, Nullable\<Int32\>, Nullable\<Int32\>, Boolean)**

```csharp
public TemporaryEndpointDefinition(string tag, Nullable<int> concurrentMessageLimit, Nullable<int> prefetchCount, bool configureConsumeTopology)
```

#### Parameters

`tag` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`concurrentMessageLimit` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`prefetchCount` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`configureConsumeTopology` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **GetEndpointName(IEndpointNameFormatter)**

```csharp
public string GetEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

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

`context` [IRegistrationContext](../masstransit/iregistrationcontext)<br/>
