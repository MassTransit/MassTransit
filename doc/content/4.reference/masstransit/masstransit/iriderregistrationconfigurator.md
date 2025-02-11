---

title: IRiderRegistrationConfigurator

---

# IRiderRegistrationConfigurator

Namespace: MassTransit

```csharp
public interface IRiderRegistrationConfigurator : IRegistrationConfigurator, IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable
```

Implements [IRegistrationConfigurator](../masstransit/iregistrationconfigurator), IServiceCollection, [IList\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Registrar**

```csharp
public abstract IContainerRegistrar Registrar { get; }
```

#### Property Value

[IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

## Methods

### **TryAddScoped\<TRider, TService\>(Func\<TRider, IServiceProvider, TService\>)**

```csharp
void TryAddScoped<TRider, TService>(Func<TRider, IServiceProvider, TService> factory)
```

#### Type Parameters

`TRider`<br/>

`TService`<br/>

#### Parameters

`factory` [Func\<TRider, IServiceProvider, TService\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

### **SetRiderFactory\<TRider\>(IRegistrationRiderFactory\<TRider\>)**

Add the rider to the container, configured properly

```csharp
void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
```

#### Type Parameters

`TRider`<br/>

#### Parameters

`riderFactory` [IRegistrationRiderFactory\<TRider\>](../masstransit-dependencyinjection/iregistrationriderfactory-1)<br/>
