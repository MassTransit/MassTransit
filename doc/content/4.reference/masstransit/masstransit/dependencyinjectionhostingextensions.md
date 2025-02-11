---

title: DependencyInjectionHostingExtensions

---

# DependencyInjectionHostingExtensions

Namespace: MassTransit

```csharp
public static class DependencyInjectionHostingExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionHostingExtensions](../masstransit/dependencyinjectionhostingextensions)

## Methods

### **UseMassTransit(IHostBuilder, Action\<HostBuilderContext, IBusRegistrationConfigurator\>)**

Adds MassTransit and its dependencies and allows consumers, sagas, and activities to be configured

```csharp
public static IHostBuilder UseMassTransit(IHostBuilder hostBuilder, Action<HostBuilderContext, IBusRegistrationConfigurator> configure)
```

#### Parameters

`hostBuilder` IHostBuilder<br/>

`configure` [Action\<HostBuilderContext, IBusRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

IHostBuilder<br/>

### **UseMassTransit\<TBus\>(IHostBuilder, Action\<HostBuilderContext, IBusRegistrationConfigurator\<TBus\>\>)**

Configure a MassTransit MultiBus instance, using the specified  bus type, which must inherit directly from .
 A dynamic type will be created to support the bus instance, which will be initialized when the  type is retrieved
 from the container.

```csharp
public static IHostBuilder UseMassTransit<TBus>(IHostBuilder hostBuilder, Action<HostBuilderContext, IBusRegistrationConfigurator<TBus>> configure)
```

#### Type Parameters

`TBus`<br/>

#### Parameters

`hostBuilder` IHostBuilder<br/>

`configure` [Action\<HostBuilderContext, IBusRegistrationConfigurator\<TBus\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

IHostBuilder<br/>

### **UseMassTransit\<TBus, TBusInstance\>(IHostBuilder, Action\<HostBuilderContext, IBusRegistrationConfigurator\<TBus\>\>)**

Configure a MassTransit bus instance, using the specified  bus type, which must inherit directly from .
 A type that implements  is required, specified by the  parameter.

```csharp
public static IHostBuilder UseMassTransit<TBus, TBusInstance>(IHostBuilder hostBuilder, Action<HostBuilderContext, IBusRegistrationConfigurator<TBus>> configure)
```

#### Type Parameters

`TBus`<br/>

`TBusInstance`<br/>

#### Parameters

`hostBuilder` IHostBuilder<br/>

`configure` [Action\<HostBuilderContext, IBusRegistrationConfigurator\<TBus\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

IHostBuilder<br/>

### **UseMediator(IHostBuilder, Action\<HostBuilderContext, IMediatorRegistrationConfigurator\>)**

Adds the MassTransit Mediator to the host, and allows consumers, sagas, and activities (which are not supported
 by the Mediator) to be configured.

```csharp
public static IHostBuilder UseMediator(IHostBuilder hostBuilder, Action<HostBuilderContext, IMediatorRegistrationConfigurator> configure)
```

#### Parameters

`hostBuilder` IHostBuilder<br/>

`configure` [Action\<HostBuilderContext, IMediatorRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

IHostBuilder<br/>
