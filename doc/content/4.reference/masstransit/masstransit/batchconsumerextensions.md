---

title: BatchConsumerExtensions

---

# BatchConsumerExtensions

Namespace: MassTransit

```csharp
public static class BatchConsumerExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchConsumerExtensions](../masstransit/batchconsumerextensions)

## Methods

### **Batch\<TMessage\>(IReceiveEndpointConfigurator, Action\<IBatchConfigurator\<TMessage\>\>)**

Configure a Batch&lt;&gt; consumer, which allows messages to be collected into an array and consumed
 at once. This feature is experimental, but often requested. Be sure to configure the transport with sufficient concurrent message
 capacity (prefetch, etc.) so that a batch can actually complete without always reaching the time limit.

```csharp
public static void Batch<TMessage>(IReceiveEndpointConfigurator configurator, Action<IBatchConfigurator<TMessage>> configure)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`configure` [Action\<IBatchConfigurator\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Consumer\<TConsumer, TMessage\>(IBatchConfigurator\<TMessage\>, Func\<TConsumer\>)**

Connect a consumer with a consumer factory method

```csharp
public static void Consumer<TConsumer, TMessage>(IBatchConfigurator<TMessage> configurator, Func<TConsumer> consumerFactoryMethod)
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IBatchConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ibatchconfigurator-1)<br/>

`consumerFactoryMethod` [Func\<TConsumer\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

### **Consumer\<TConsumer, TMessage\>(IBatchConfigurator\<TMessage\>, IConsumerFactory\<TConsumer\>)**

Connect a consumer with a consumer factory method

```csharp
public static void Consumer<TConsumer, TMessage>(IBatchConfigurator<TMessage> configurator, IConsumerFactory<TConsumer> consumerFactory)
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IBatchConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ibatchconfigurator-1)<br/>

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>
