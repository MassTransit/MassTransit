---

title: ConsumerExtensions

---

# ConsumerExtensions

Namespace: MassTransit

```csharp
public static class ConsumerExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerExtensions](../masstransit/consumerextensions)

## Methods

### **Consumer\<TConsumer\>(IReceiveEndpointConfigurator, IConsumerFactory\<TConsumer\>, Action\<IConsumerConfigurator\<TConsumer\>\>)**

Connect a consumer to the receiving endpoint

```csharp
public static void Consumer<TConsumer>(IReceiveEndpointConfigurator configurator, IConsumerFactory<TConsumer> consumerFactory, Action<IConsumerConfigurator<TConsumer>> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`configure` [Action\<IConsumerConfigurator\<TConsumer\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Optional, configure the consumer

### **ConnectConsumer\<TConsumer\>(IConsumePipeConnector, IConsumerFactory\<TConsumer\>, IPipeSpecification`1[])**

Connect a consumer to the bus instance's default endpoint

```csharp
public static ConnectHandle ConnectConsumer<TConsumer>(IConsumePipeConnector connector, IConsumerFactory<TConsumer> consumerFactory, IPipeSpecification`1[] pipeSpecifications)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`connector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`pipeSpecifications` [IPipeSpecification`1[]](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Consumer\<TConsumer\>(IReceiveEndpointConfigurator, Action\<IConsumerConfigurator\<TConsumer\>\>)**

Subscribes a consumer with a default constructor to the endpoint

```csharp
public static void Consumer<TConsumer>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<TConsumer>> configure)
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`configure` [Action\<IConsumerConfigurator\<TConsumer\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConnectConsumer\<TConsumer\>(IConsumePipeConnector, IPipeSpecification`1[])**

Subscribe a consumer with a default constructor to the bus's default endpoint

```csharp
public static ConnectHandle ConnectConsumer<TConsumer>(IConsumePipeConnector connector, IPipeSpecification`1[] pipeSpecifications)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`connector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`pipeSpecifications` [IPipeSpecification`1[]](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Consumer\<TConsumer\>(IReceiveEndpointConfigurator, Func\<TConsumer\>, Action\<IConsumerConfigurator\<TConsumer\>\>)**

Connect a consumer with a consumer factory method

```csharp
public static void Consumer<TConsumer>(IReceiveEndpointConfigurator configurator, Func<TConsumer> consumerFactoryMethod, Action<IConsumerConfigurator<TConsumer>> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`consumerFactoryMethod` [Func\<TConsumer\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`configure` [Action\<IConsumerConfigurator\<TConsumer\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConnectConsumer\<TConsumer\>(IConsumePipeConnector, Func\<TConsumer\>, IPipeSpecification`1[])**

Subscribe a consumer with a consumer factor method to the bus's default endpoint

```csharp
public static ConnectHandle ConnectConsumer<TConsumer>(IConsumePipeConnector connector, Func<TConsumer> consumerFactoryMethod, IPipeSpecification`1[] pipeSpecifications)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`connector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`consumerFactoryMethod` [Func\<TConsumer\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`pipeSpecifications` [IPipeSpecification`1[]](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Consumer(IReceiveEndpointConfigurator, Type, Func\<Type, Object\>)**

Connect a consumer with a consumer type and object factory method for the consumer (used by containers mostly)

```csharp
public static void Consumer(IReceiveEndpointConfigurator configurator, Type consumerType, Func<Type, object> consumerFactory)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`consumerType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`consumerFactory` [Func\<Type, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **ConnectConsumer(IConsumePipeConnector, Type, Func\<Type, Object\>)**

Connect a consumer with a consumer type and object factory method for the consumer

```csharp
public static ConnectHandle ConnectConsumer(IConsumePipeConnector connector, Type consumerType, Func<Type, object> objectFactory)
```

#### Parameters

`connector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`consumerType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`objectFactory` [Func\<Type, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
