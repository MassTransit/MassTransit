---

title: ServiceCollectionRiderConfigurator

---

# ServiceCollectionRiderConfigurator

Namespace: MassTransit.Configuration

```csharp
public class ServiceCollectionRiderConfigurator : RegistrationConfigurator, IRegistrationConfigurator, IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable, IRiderRegistrationConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationConfigurator](../masstransit-configuration/registrationconfigurator) → [ServiceCollectionRiderConfigurator](../masstransit-configuration/servicecollectionriderconfigurator)<br/>
Implements [IRegistrationConfigurator](../masstransit/iregistrationconfigurator), IServiceCollection, [IList\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable), [IRiderRegistrationConfigurator](../masstransit/iriderregistrationconfigurator)

## Properties

### **Registrar**

```csharp
public IContainerRegistrar Registrar { get; }
```

#### Property Value

[IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **IsReadOnly**

```csharp
public bool IsReadOnly { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Item**

```csharp
public ServiceDescriptor Item { get; set; }
```

#### Property Value

ServiceDescriptor<br/>

## Constructors

### **ServiceCollectionRiderConfigurator(IServiceCollection, IContainerRegistrar)**

```csharp
public ServiceCollectionRiderConfigurator(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

## Methods

### **TryAddScoped\<TRider, TService\>(Func\<TRider, IServiceProvider, TService\>)**

```csharp
public void TryAddScoped<TRider, TService>(Func<TRider, IServiceProvider, TService> factory)
```

#### Type Parameters

`TRider`<br/>

`TService`<br/>

#### Parameters

`factory` [Func\<TRider, IServiceProvider, TService\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

### **SetRiderFactory\<TRider\>(IRegistrationRiderFactory\<TRider\>)**

```csharp
public void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
```

#### Type Parameters

`TRider`<br/>

#### Parameters

`riderFactory` [IRegistrationRiderFactory\<TRider\>](../masstransit-dependencyinjection/iregistrationriderfactory-1)<br/>

### **ThrowIfAlreadyConfigured(Type)**

```csharp
protected void ThrowIfAlreadyConfigured(Type serviceType)
```

#### Parameters

`serviceType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
