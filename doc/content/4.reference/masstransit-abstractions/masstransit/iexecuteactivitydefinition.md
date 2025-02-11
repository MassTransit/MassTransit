---

title: IExecuteActivityDefinition

---

# IExecuteActivityDefinition

Namespace: MassTransit

```csharp
public interface IExecuteActivityDefinition : IDefinition
```

Implements [IDefinition](../masstransit/idefinition)

## Properties

### **ActivityType**

The Activity type

```csharp
public abstract Type ActivityType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **ArgumentType**

The argument type

```csharp
public abstract Type ArgumentType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **ExecuteEndpointDefinition**

```csharp
public abstract IEndpointDefinition ExecuteEndpointDefinition { get; }
```

#### Property Value

[IEndpointDefinition](../masstransit/iendpointdefinition)<br/>

## Methods

### **GetExecuteEndpointName(IEndpointNameFormatter)**

Return the endpoint name for the execute activity

```csharp
string GetExecuteEndpointName(IEndpointNameFormatter formatter)
```

#### Parameters

`formatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
