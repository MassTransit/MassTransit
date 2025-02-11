---

title: ServiceCollectionBusConfigurator

---

# ServiceCollectionBusConfigurator

Namespace: MassTransit.Configuration

```csharp
public class ServiceCollectionBusConfigurator : RegistrationConfigurator, IRegistrationConfigurator, IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable, IBusRegistrationConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationConfigurator](../masstransit-configuration/registrationconfigurator) → [ServiceCollectionBusConfigurator](../masstransit-configuration/servicecollectionbusconfigurator)<br/>
Implements [IRegistrationConfigurator](../masstransit/iregistrationconfigurator), IServiceCollection, [IList\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable), [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)

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

### **ServiceCollectionBusConfigurator(IServiceCollection)**

```csharp
public ServiceCollectionBusConfigurator(IServiceCollection collection)
```

#### Parameters

`collection` IServiceCollection<br/>

## Methods

### **AddBus(Func\<IBusRegistrationContext, IBusControl\>)**

#### Caution

Use 'Using[TransportName]' instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
```

#### Parameters

`busFactory` [Func\<IBusRegistrationContext, IBusControl\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **SetBusFactory\<T\>(T)**

```csharp
public void SetBusFactory<T>(T busFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`busFactory` T<br/>

### **AddRider(Action\<IRiderRegistrationConfigurator\>)**

```csharp
public void AddRider(Action<IRiderRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IRiderRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **AddConfigureEndpointsCallback(ConfigureEndpointsCallback)**

```csharp
public void AddConfigureEndpointsCallback(ConfigureEndpointsCallback callback)
```

#### Parameters

`callback` [ConfigureEndpointsCallback](../masstransit/configureendpointscallback)<br/>

### **AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback)**

```csharp
public void AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback callback)
```

#### Parameters

`callback` [ConfigureEndpointsProviderCallback](../masstransit/configureendpointsprovidercallback)<br/>

### **SetRequestClientFactory(Func\<IBus, RequestTimeout, IClientFactory\>)**

```csharp
public void SetRequestClientFactory(Func<IBus, RequestTimeout, IClientFactory> clientFactory)
```

#### Parameters

`clientFactory` [Func\<IBus, RequestTimeout, IClientFactory\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>
