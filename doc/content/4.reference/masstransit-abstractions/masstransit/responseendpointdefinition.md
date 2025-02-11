---

title: ResponseEndpointDefinition

---

# ResponseEndpointDefinition

Namespace: MassTransit

Specifies a temporary endpoint, with the prefix "response"

```csharp
public class ResponseEndpointDefinition : TemporaryEndpointDefinition, IEndpointDefinition
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TemporaryEndpointDefinition](../masstransit/temporaryendpointdefinition) → [ResponseEndpointDefinition](../masstransit/responseendpointdefinition)<br/>
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

### **ResponseEndpointDefinition()**

```csharp
public ResponseEndpointDefinition()
```
