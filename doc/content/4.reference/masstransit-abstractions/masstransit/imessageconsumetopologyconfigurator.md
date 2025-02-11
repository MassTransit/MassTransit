---

title: IMessageConsumeTopologyConfigurator

---

# IMessageConsumeTopologyConfigurator

Namespace: MassTransit

```csharp
public interface IMessageConsumeTopologyConfigurator : ISpecification
```

Implements [ISpecification](../masstransit/ispecification)

## Properties

### **ConfigureConsumeTopology**

Specify whether the broker topology should be configured for this message type
 (defaults to true)

```csharp
public abstract bool ConfigureConsumeTopology { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **TryAddConvention(IConsumeTopologyConvention)**

```csharp
bool TryAddConvention(IConsumeTopologyConvention convention)
```

#### Parameters

`convention` [IConsumeTopologyConvention](../masstransit-configuration/iconsumetopologyconvention)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
