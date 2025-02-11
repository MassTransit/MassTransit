---

title: ExcludeFromConfigureEndpointsAttribute

---

# ExcludeFromConfigureEndpointsAttribute

Namespace: MassTransit

When added to a consuming type (consumer, saga, activity, etc), prevents
 MassTransit from configuring endpoint for it when ConfigureEndpoints called

```csharp
public class ExcludeFromConfigureEndpointsAttribute : Attribute
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://learn.microsoft.com/en-us/dotnet/api/system.attribute) → [ExcludeFromConfigureEndpointsAttribute](../masstransit/excludefromconfigureendpointsattribute)

## Properties

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **ExcludeFromConfigureEndpointsAttribute()**

```csharp
public ExcludeFromConfigureEndpointsAttribute()
```
