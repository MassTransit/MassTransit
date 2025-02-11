---

title: IFutureDefinition

---

# IFutureDefinition

Namespace: MassTransit

```csharp
public interface IFutureDefinition : IDefinition
```

Implements [IDefinition](../masstransit/idefinition)

## Properties

### **FutureType**

```csharp
public abstract Type FutureType { get; }
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

Return the endpoint name for the future

```csharp
string GetEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
