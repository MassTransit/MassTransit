---

title: IEndpointRegistration

---

# IEndpointRegistration

Namespace: MassTransit.Configuration

```csharp
public interface IEndpointRegistration : IRegistration
```

Implements [IRegistration](../masstransit-configuration/iregistration)

## Methods

### **GetDefinition(IServiceProvider)**

```csharp
IEndpointDefinition GetDefinition(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>
