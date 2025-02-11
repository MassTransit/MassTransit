---

title: DependencyInjectionEndpointRegistrationExtensions

---

# DependencyInjectionEndpointRegistrationExtensions

Namespace: MassTransit.Configuration

```csharp
public static class DependencyInjectionEndpointRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionEndpointRegistrationExtensions](../masstransit-configuration/dependencyinjectionendpointregistrationextensions)

## Methods

### **RegisterEndpoint\<TDefinition, T\>(IServiceCollection, IRegistration, IEndpointSettings\<IEndpointDefinition\<T\>\>)**

```csharp
public static IEndpointRegistration RegisterEndpoint<TDefinition, T>(IServiceCollection collection, IRegistration registration, IEndpointSettings<IEndpointDefinition<T>> settings)
```

#### Type Parameters

`TDefinition`<br/>

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registration` [IRegistration](../masstransit-configuration/iregistration)<br/>

`settings` [IEndpointSettings\<IEndpointDefinition\<T\>\>](../../masstransit-abstractions/masstransit/iendpointsettings-1)<br/>

#### Returns

[IEndpointRegistration](../masstransit-configuration/iendpointregistration)<br/>

### **RegisterEndpoint\<TDefinition, T\>(IServiceCollection, IContainerRegistrar, IRegistration, IEndpointSettings\<IEndpointDefinition\<T\>\>)**

```csharp
public static IEndpointRegistration RegisterEndpoint<TDefinition, T>(IServiceCollection collection, IContainerRegistrar registrar, IRegistration registration, IEndpointSettings<IEndpointDefinition<T>> settings)
```

#### Type Parameters

`TDefinition`<br/>

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`registration` [IRegistration](../masstransit-configuration/iregistration)<br/>

`settings` [IEndpointSettings\<IEndpointDefinition\<T\>\>](../../masstransit-abstractions/masstransit/iendpointsettings-1)<br/>

#### Returns

[IEndpointRegistration](../masstransit-configuration/iendpointregistration)<br/>

### **RegisterEndpoint(IServiceCollection, Type)**

```csharp
public static IEndpointRegistration RegisterEndpoint(IServiceCollection collection, Type endpointDefinitionType)
```

#### Parameters

`collection` IServiceCollection<br/>

`endpointDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IEndpointRegistration](../masstransit-configuration/iendpointregistration)<br/>

### **RegisterEndpoint(IServiceCollection, IContainerRegistrar, Type)**

```csharp
public static IEndpointRegistration RegisterEndpoint(IServiceCollection collection, IContainerRegistrar registrar, Type endpointDefinitionType)
```

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`endpointDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IEndpointRegistration](../masstransit-configuration/iendpointregistration)<br/>
