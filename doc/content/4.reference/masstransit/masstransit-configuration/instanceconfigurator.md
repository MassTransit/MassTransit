---

title: InstanceConfigurator

---

# InstanceConfigurator

Namespace: MassTransit.Configuration

```csharp
public class InstanceConfigurator : IInstanceConfigurator, IConsumeConfigurator, IReceiveEndpointSpecification, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstanceConfigurator](../masstransit-configuration/instanceconfigurator)<br/>
Implements [IInstanceConfigurator](../../masstransit-abstractions/masstransit/iinstanceconfigurator), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IReceiveEndpointSpecification](../../masstransit-abstractions/masstransit/ireceiveendpointspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **InstanceConfigurator(Object)**

```csharp
public InstanceConfigurator(object instance)
```

#### Parameters

`instance` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Methods

### **Configure(IReceiveEndpointBuilder)**

```csharp
public void Configure(IReceiveEndpointBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBuilder](../../masstransit-abstractions/masstransit-configuration/ireceiveendpointbuilder)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
