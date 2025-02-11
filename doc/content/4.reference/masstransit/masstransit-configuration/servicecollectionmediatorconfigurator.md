---

title: ServiceCollectionMediatorConfigurator

---

# ServiceCollectionMediatorConfigurator

Namespace: MassTransit.Configuration

```csharp
public class ServiceCollectionMediatorConfigurator : RegistrationConfigurator, IRegistrationConfigurator, IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable, IMediatorRegistrationConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationConfigurator](../masstransit-configuration/registrationconfigurator) → [ServiceCollectionMediatorConfigurator](../masstransit-configuration/servicecollectionmediatorconfigurator)<br/>
Implements [IRegistrationConfigurator](../masstransit/iregistrationconfigurator), IServiceCollection, [IList\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable), [IMediatorRegistrationConfigurator](../masstransit/imediatorregistrationconfigurator)

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

### **ServiceCollectionMediatorConfigurator(IServiceCollection, Uri)**

```csharp
public ServiceCollectionMediatorConfigurator(IServiceCollection collection, Uri baseAddress)
```

#### Parameters

`collection` IServiceCollection<br/>

`baseAddress` Uri<br/>

## Methods

### **ConfigureMediator(Action\<IMediatorRegistrationContext, IMediatorConfigurator\>)**

```csharp
public void ConfigureMediator(Action<IMediatorRegistrationContext, IMediatorConfigurator> configure)
```

#### Parameters

`configure` [Action\<IMediatorRegistrationContext, IMediatorConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
