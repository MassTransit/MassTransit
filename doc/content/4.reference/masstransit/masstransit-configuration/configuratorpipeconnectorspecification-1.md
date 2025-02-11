---

title: ConfiguratorPipeConnectorSpecification<TContext>

---

# ConfiguratorPipeConnectorSpecification\<TContext\>

Namespace: MassTransit.Configuration

```csharp
public class ConfiguratorPipeConnectorSpecification<TContext> : IPipeConfigurator<TContext>, IPipeConnectorSpecification, ISpecification
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConfiguratorPipeConnectorSpecification\<TContext\>](../masstransit-configuration/configuratorpipeconnectorspecification-1)<br/>
Implements [IPipeConfigurator\<TContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IPipeConnectorSpecification](../masstransit/ipipeconnectorspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ConfiguratorPipeConnectorSpecification()**

```csharp
public ConfiguratorPipeConnectorSpecification()
```

## Methods

### **AddPipeSpecification(IPipeSpecification\<TContext\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<TContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<TContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Connect(IPipeConnector)**

```csharp
public void Connect(IPipeConnector connector)
```

#### Parameters

`connector` [IPipeConnector](../masstransit-middleware/ipipeconnector)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
