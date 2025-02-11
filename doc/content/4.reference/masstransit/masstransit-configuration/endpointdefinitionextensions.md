---

title: EndpointDefinitionExtensions

---

# EndpointDefinitionExtensions

Namespace: MassTransit.Configuration

```csharp
public static class EndpointDefinitionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointDefinitionExtensions](../masstransit-configuration/endpointdefinitionextensions)

## Methods

### **Combine(IEnumerable\<IEndpointDefinition\>, IRegistrationContext)**

```csharp
public static IEndpointDefinition Combine(IEnumerable<IEndpointDefinition> definitions, IRegistrationContext context)
```

#### Parameters

`definitions` [IEnumerable\<IEndpointDefinition\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

#### Returns

[IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>
