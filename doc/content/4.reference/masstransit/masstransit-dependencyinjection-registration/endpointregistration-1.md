---

title: EndpointRegistration<T>

---

# EndpointRegistration\<T\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class EndpointRegistration<T> : IEndpointRegistration, IRegistration
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointRegistration\<T\>](../masstransit-dependencyinjection-registration/endpointregistration-1)<br/>
Implements [IEndpointRegistration](../masstransit-configuration/iendpointregistration), [IRegistration](../masstransit-configuration/iregistration)

## Properties

### **Type**

```csharp
public Type Type { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **IncludeInConfigureEndpoints**

```csharp
public bool IncludeInConfigureEndpoints { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **EndpointRegistration(IRegistration, IContainerSelector)**

```csharp
public EndpointRegistration(IRegistration registration, IContainerSelector selector)
```

#### Parameters

`registration` [IRegistration](../masstransit-configuration/iregistration)<br/>

`selector` [IContainerSelector](../masstransit-configuration/icontainerselector)<br/>

## Methods

### **GetDefinition(IServiceProvider)**

```csharp
public IEndpointDefinition GetDefinition(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>
