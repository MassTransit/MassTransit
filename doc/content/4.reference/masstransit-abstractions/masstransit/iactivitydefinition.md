---

title: IActivityDefinition

---

# IActivityDefinition

Namespace: MassTransit

```csharp
public interface IActivityDefinition : IExecuteActivityDefinition, IDefinition
```

Implements [IExecuteActivityDefinition](../masstransit/iexecuteactivitydefinition), [IDefinition](../masstransit/idefinition)

## Properties

### **LogType**

The log type

```csharp
public abstract Type LogType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **CompensateEndpointDefinition**

```csharp
public abstract IEndpointDefinition CompensateEndpointDefinition { get; }
```

#### Property Value

[IEndpointDefinition](../masstransit/iendpointdefinition)<br/>

## Methods

### **GetCompensateEndpointName(IEndpointNameFormatter)**

Return the endpoint name for the compensate activity

```csharp
string GetCompensateEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
