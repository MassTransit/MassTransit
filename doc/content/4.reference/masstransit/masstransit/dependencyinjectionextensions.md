---

title: DependencyInjectionExtensions

---

# DependencyInjectionExtensions

Namespace: MassTransit

```csharp
public static class DependencyInjectionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionExtensions](../masstransit/dependencyinjectionextensions)

## Methods

### **UseServiceScope(IConsumePipeConfigurator, IRegistrationContext)**

Creates a single scope for the receive endpoint that is used by all consumers, sagas, messages, etc.

```csharp
public static void UseServiceScope(IConsumePipeConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **UseServiceScope(IConsumePipeConfigurator, IServiceProvider)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Creates a single scope for the receive endpoint that is used by all consumers, sagas, messages, etc.

```csharp
public static void UseServiceScope(IConsumePipeConfigurator configurator, IServiceProvider serviceProvider)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`serviceProvider` IServiceProvider<br/>

### **UseMessageScope(IConsumePipeConfigurator, IRegistrationContext)**

Creates a scope for each message type, compatible with UseMessageRetry and UseInMemoryOutbox

```csharp
public static void UseMessageScope(IConsumePipeConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **UseMessageScope(IConsumePipeConfigurator, IServiceProvider)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Creates a scope for each message type, compatible with UseMessageRetry and UseInMemoryOutbox

```csharp
public static void UseMessageScope(IConsumePipeConfigurator configurator, IServiceProvider serviceProvider)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`serviceProvider` IServiceProvider<br/>

### **RegisterInMemorySagaRepository\<T\>(IServiceCollection)**

Register the InMemory saga repository for the specified saga type

```csharp
public static void RegisterInMemorySagaRepository<T>(IServiceCollection collection)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

### **CreateRequestClient\<T\>(IServiceProvider, RequestTimeout)**

Create a request client, using the specified service address, using the  from the container.

```csharp
public static IRequestClient<T> CreateRequestClient<T>(IServiceProvider provider, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`provider` IServiceProvider<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The default timeout for requests

#### Returns

[IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **CreateRequestClient\<T\>(IServiceProvider, Uri, RequestTimeout)**

Create a request client, using the specified service address, using the  from the container.

```csharp
public static IRequestClient<T> CreateRequestClient<T>(IServiceProvider provider, Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`provider` IServiceProvider<br/>

`destinationAddress` Uri<br/>
The destination service address

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The default timeout for requests

#### Returns

[IRequestClient\<T\>](../../masstransit-abstractions/masstransit/irequestclient-1)<br/>

### **AddGenericRequestClient(IServiceCollection)**

#### Caution

Remove, the generic request client is automatically registered. Visit https://masstransit.io/obsolete for details.

---

Registers a generic request client provider in the container, which will be used for any
 client that is not explicitly registered using AddRequestClient.

```csharp
public static IServiceCollection AddGenericRequestClient(IServiceCollection collection)
```

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>
