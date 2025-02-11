---

title: IConsumerDefinition

---

# IConsumerDefinition

Namespace: MassTransit

```csharp
public interface IConsumerDefinition : IDefinition
```

Implements [IDefinition](../masstransit/idefinition)

## Properties

### **ConsumerType**

The consumer type

```csharp
public abstract Type ConsumerType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **EndpointDefinition**

```csharp
public abstract IEndpointDefinition EndpointDefinition { get; }
```

#### Property Value

[IEndpointDefinition](../masstransit/iendpointdefinition)<br/>

## Methods

### **GetEndpointName(IEndpointNameFormatter)**

Return the endpoint name for the consumer, using the specified formatter if necessary.

```csharp
string GetEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
