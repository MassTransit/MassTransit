---

title: RoutingSlipConfigurator

---

# RoutingSlipConfigurator

Namespace: MassTransit.Configuration

```csharp
public class RoutingSlipConfigurator : IRoutingSlipConfigurator, IConsumeConfigurator, IPipeConfigurator<ConsumeContext<RoutingSlip>>, IBuildPipeConfigurator<ConsumeContext<RoutingSlip>>, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipConfigurator](../masstransit-configuration/routingslipconfigurator)<br/>
Implements [IRoutingSlipConfigurator](../../masstransit-abstractions/masstransit/iroutingslipconfigurator), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IPipeConfigurator\<ConsumeContext\<RoutingSlip\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IBuildPipeConfigurator\<ConsumeContext\<RoutingSlip\>\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **RoutingSlipConfigurator()**

```csharp
public RoutingSlipConfigurator()
```

## Methods

### **Build()**

```csharp
public IPipe<ConsumeContext<RoutingSlip>> Build()
```

#### Returns

[IPipe\<ConsumeContext\<RoutingSlip\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\<RoutingSlip\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext<RoutingSlip>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<RoutingSlip\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>
