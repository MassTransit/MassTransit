---

title: ConsumerTestHarness<TConsumer>

---

# ConsumerTestHarness\<TConsumer\>

Namespace: MassTransit.Testing

```csharp
public class ConsumerTestHarness<TConsumer> : IConsumerTestHarness<TConsumer>
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerTestHarness\<TConsumer\>](../masstransit-testing/consumertestharness-1)<br/>
Implements [IConsumerTestHarness\<TConsumer\>](../masstransit-testing/iconsumertestharness-1)

## Properties

### **Consumed**

```csharp
public IReceivedMessageList Consumed { get; }
```

#### Property Value

[IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)<br/>

## Constructors

### **ConsumerTestHarness(BusTestHarness, IConsumerFactory\<TConsumer\>, Action\<IConsumerConfigurator\<TConsumer\>\>, String)**

```csharp
public ConsumerTestHarness(BusTestHarness testHarness, IConsumerFactory<TConsumer> consumerFactory, Action<IConsumerConfigurator<TConsumer>> configure, string queueName)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`configure` [Action\<IConsumerConfigurator\<TConsumer\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumerTestHarness(BusTestHarness, IConsumerFactory\<TConsumer\>, String)**

```csharp
public ConsumerTestHarness(BusTestHarness testHarness, IConsumerFactory<TConsumer> consumerFactory, string queueName)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConsumerTestHarness(BusTestHarness, IConsumerFactory\<TConsumer\>, Action\<IConsumerConfigurator\<TConsumer\>\>)**

```csharp
public ConsumerTestHarness(BusTestHarness testHarness, IConsumerFactory<TConsumer> consumerFactory, Action<IConsumerConfigurator<TConsumer>> configure)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`configure` [Action\<IConsumerConfigurator\<TConsumer\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConsumerTestHarness(BusTestHarness, IConsumerFactory\<TConsumer\>)**

```csharp
public ConsumerTestHarness(BusTestHarness testHarness, IConsumerFactory<TConsumer> consumerFactory)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

## Methods

### **ConfigureReceiveEndpoint(IReceiveEndpointConfigurator)**

```csharp
protected void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator, String)**

```csharp
protected void ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator configurator, string queueName)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
