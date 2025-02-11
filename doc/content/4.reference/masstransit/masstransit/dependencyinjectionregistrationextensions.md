---

title: DependencyInjectionRegistrationExtensions

---

# DependencyInjectionRegistrationExtensions

Namespace: MassTransit

Standard registration extensions, which are used to configure consumers, sagas, and activities on receive endpoints from a
 dependency injection container.

```csharp
public static class DependencyInjectionRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionRegistrationExtensions](../masstransit/dependencyinjectionregistrationextensions)

## Methods

### **AddMassTransit(IServiceCollection, Action\<IBusRegistrationConfigurator\>)**

Adds MassTransit and its dependencies to the , and allows consumers, sagas, and activities to be configured

```csharp
public static IServiceCollection AddMassTransit(IServiceCollection collection, Action<IBusRegistrationConfigurator> configure)
```

#### Parameters

`collection` IServiceCollection<br/>

`configure` [Action\<IBusRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

IServiceCollection<br/>

### **AddMediator(IServiceCollection, Uri, Action\<IMediatorRegistrationConfigurator\>)**

Adds the MassTransit Mediator to the , and allows consumers, sagas, and activities (which are not supported
 by the Mediator) to be configured.

```csharp
public static IServiceCollection AddMediator(IServiceCollection collection, Uri baseAddress, Action<IMediatorRegistrationConfigurator> configure)
```

#### Parameters

`collection` IServiceCollection<br/>

`baseAddress` Uri<br/>

`configure` [Action\<IMediatorRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

IServiceCollection<br/>

### **AddMediator(IServiceCollection, Action\<IMediatorRegistrationConfigurator\>)**

Adds the MassTransit Mediator to the , and allows consumers, sagas, and activities (which are not supported
 by the Mediator) to be configured.

```csharp
public static IServiceCollection AddMediator(IServiceCollection collection, Action<IMediatorRegistrationConfigurator> configure)
```

#### Parameters

`collection` IServiceCollection<br/>

`configure` [Action\<IMediatorRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

IServiceCollection<br/>

### **AddMassTransit\<TBus, TBusInstance\>(IServiceCollection, Action\<IBusRegistrationConfigurator\<TBus\>\>)**

Configure a MassTransit bus instance, using the specified  bus type, which must inherit directly from .
 A type that implements  is required, specified by the  parameter.

```csharp
public static IServiceCollection AddMassTransit<TBus, TBusInstance>(IServiceCollection collection, Action<IBusRegistrationConfigurator<TBus>> configure)
```

#### Type Parameters

`TBus`<br/>

`TBusInstance`<br/>

#### Parameters

`collection` IServiceCollection<br/>
The service collection

`configure` [Action\<IBusRegistrationConfigurator\<TBus\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Bus instance configuration method

#### Returns

IServiceCollection<br/>

### **AddMassTransit\<TBus\>(IServiceCollection, Action\<IBusRegistrationConfigurator\<TBus\>\>)**

Configure a MassTransit MultiBus instance, using the specified  bus type, which must inherit directly from .
 A dynamic type will be created to support the bus instance, which will be initialized when the  type is retrieved
 from the container.

```csharp
public static IServiceCollection AddMassTransit<TBus>(IServiceCollection collection, Action<IBusRegistrationConfigurator<TBus>> configure)
```

#### Type Parameters

`TBus`<br/>

#### Parameters

`collection` IServiceCollection<br/>
The service collection

`configure` [Action\<IBusRegistrationConfigurator\<TBus\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Bus instance configuration method

#### Returns

IServiceCollection<br/>

### **RemoveMassTransitHostedService(IServiceCollection)**

In some situations, it may be necessary to Remove the MassTransitHostedService from the container, such as
 when using older versions of the Azure Functions runtime.

```csharp
public static IServiceCollection RemoveMassTransitHostedService(IServiceCollection services)
```

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **RemoveHostedService\<T\>(IServiceCollection)**

Remove the specified hosted service from the service collection

```csharp
public static IServiceCollection RemoveHostedService<T>(IServiceCollection services)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **ReplaceScoped\<TService, TImplementation\>(IServiceCollection)**

Replace a scoped service registration with a new one

```csharp
public static void ReplaceScoped<TService, TImplementation>(IServiceCollection services)
```

#### Type Parameters

`TService`<br/>

`TImplementation`<br/>

#### Parameters

`services` IServiceCollection<br/>

### **RemoveMassTransit(IServiceCollection)**

```csharp
internal static void RemoveMassTransit(IServiceCollection collection)
```

#### Parameters

`collection` IServiceCollection<br/>
