---

title: BusFactoryExtensions

---

# BusFactoryExtensions

Namespace: MassTransit

```csharp
public static class BusFactoryExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusFactoryExtensions](../masstransit/busfactoryextensions)

## Methods

### **Build(IBusFactory, IBusConfiguration, IEnumerable\<ISpecification\>)**

```csharp
public static IBusControl Build(IBusFactory factory, IBusConfiguration busConfiguration, IEnumerable<ISpecification> dependencies)
```

#### Parameters

`factory` [IBusFactory](../masstransit/ibusfactory)<br/>

`busConfiguration` [IBusConfiguration](../masstransit-configuration/ibusconfiguration)<br/>

`dependencies` [IEnumerable\<ISpecification\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

### **Build(IBusFactory, IBusConfiguration)**

```csharp
public static IBusControl Build(IBusFactory factory, IBusConfiguration busConfiguration)
```

#### Parameters

`factory` [IBusFactory](../masstransit/ibusfactory)<br/>

`busConfiguration` [IBusConfiguration](../masstransit-configuration/ibusconfiguration)<br/>

#### Returns

[IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>
