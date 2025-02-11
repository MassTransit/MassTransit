---

title: InMemoryConfigurationExtensions

---

# InMemoryConfigurationExtensions

Namespace: MassTransit

```csharp
public static class InMemoryConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryConfigurationExtensions](../masstransit/inmemoryconfigurationextensions)

## Methods

### **CreateUsingInMemory(IBusFactorySelector, Action\<IInMemoryBusFactoryConfigurator\>)**

Configure and create an in-memory bus

```csharp
public static IBusControl CreateUsingInMemory(IBusFactorySelector selector, Action<IInMemoryBusFactoryConfigurator> configure)
```

#### Parameters

`selector` [IBusFactorySelector](../masstransit/ibusfactoryselector)<br/>
Hang off the selector interface for visibility

`configure` [Action\<IInMemoryBusFactoryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback to configure the bus

#### Returns

[IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

### **CreateUsingInMemory(IBusFactorySelector, Uri, Action\<IInMemoryBusFactoryConfigurator\>)**

Configure and create an in-memory bus

```csharp
public static IBusControl CreateUsingInMemory(IBusFactorySelector selector, Uri baseAddress, Action<IInMemoryBusFactoryConfigurator> configure)
```

#### Parameters

`selector` [IBusFactorySelector](../masstransit/ibusfactoryselector)<br/>
Hang off the selector interface for visibility

`baseAddress` Uri<br/>
Override the default base address

`configure` [Action\<IInMemoryBusFactoryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback to configure the bus

#### Returns

[IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

### **UsingInMemory(IBusRegistrationConfigurator, Action\<IBusRegistrationContext, IInMemoryBusFactoryConfigurator\>)**

Configure MassTransit to use the In-Memory transport.

```csharp
public static void UsingInMemory(IBusRegistrationConfigurator configurator, Action<IBusRegistrationContext, IInMemoryBusFactoryConfigurator> configure)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>
The registration configurator (configured via AddMassTransit)

`configure` [Action\<IBusRegistrationContext, IInMemoryBusFactoryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
The configuration callback for the bus factory

### **UsingInMemory(IBusRegistrationConfigurator, Uri, Action\<IBusRegistrationContext, IInMemoryBusFactoryConfigurator\>)**

Configure MassTransit to use the In-Memory transport.

```csharp
public static void UsingInMemory(IBusRegistrationConfigurator configurator, Uri baseAddress, Action<IBusRegistrationContext, IInMemoryBusFactoryConfigurator> configure)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>
The registration configurator (configured via AddMassTransit)

`baseAddress` Uri<br/>
The base Address of the transport

`configure` [Action\<IBusRegistrationContext, IInMemoryBusFactoryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
The configuration callback for the bus factory
