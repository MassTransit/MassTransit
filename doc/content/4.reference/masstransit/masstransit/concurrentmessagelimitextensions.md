---

title: ConcurrentMessageLimitExtensions

---

# ConcurrentMessageLimitExtensions

Namespace: MassTransit

```csharp
public static class ConcurrentMessageLimitExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConcurrentMessageLimitExtensions](../masstransit/concurrentmessagelimitextensions)

## Methods

### **UseConcurrentMessageLimit\<TConsumer\>(IConsumerConfigurator\<TConsumer\>, Int32)**

Limits the number of concurrent messages consumed by the consumer, regardless of message type.

```csharp
public static void UseConcurrentMessageLimit<TConsumer>(IConsumerConfigurator<TConsumer> configurator, int concurrentMessageLimit)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrent message limit for all message types for the consumer

### **UseConcurrentMessageLimit\<TConsumer\>(IConsumerConfigurator\<TConsumer\>, Int32, IReceiveEndpointConfigurator, String)**

Limits the number of concurrent messages consumed by the consumer, regardless of message type.

```csharp
public static void UseConcurrentMessageLimit<TConsumer>(IConsumerConfigurator<TConsumer> configurator, int concurrentMessageLimit, IReceiveEndpointConfigurator managementEndpointConfigurator, string id)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrent message limit for all message types for the consumer

`managementEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
A management endpoint configurator to support runtime adjustment

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
An identifier for the concurrency limit to allow selective adjustment

### **UseConcurrentMessageLimit\<TSaga\>(ISagaConfigurator\<TSaga\>, Int32)**

Limits the number of concurrent messages consumed by the saga, regardless of message type.

```csharp
public static void UseConcurrentMessageLimit<TSaga>(ISagaConfigurator<TSaga> configurator, int concurrentMessageLimit)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrent message limit for all message types for the saga

### **UseConcurrentMessageLimit\<TSaga\>(ISagaConfigurator\<TSaga\>, Int32, IReceiveEndpointConfigurator, String)**

Limits the number of concurrent messages consumed by the saga, regardless of message type.

```csharp
public static void UseConcurrentMessageLimit<TSaga>(ISagaConfigurator<TSaga> configurator, int concurrentMessageLimit, IReceiveEndpointConfigurator managementEndpointConfigurator, string id)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrent message limit for all message types for the saga

`managementEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
A management endpoint configurator to support runtime adjustment

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
An identifier for the concurrency limit to allow selective adjustment

### **UseConcurrentMessageLimit\<TMessage\>(IHandlerConfigurator\<TMessage\>, Int32)**

Limits the number of concurrent messages consumed by the handler.

```csharp
public static void UseConcurrentMessageLimit<TMessage>(IHandlerConfigurator<TMessage> configurator, int concurrentMessageLimit)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrent message limit for the handler message type

### **UseConcurrentMessageLimit\<TMessage\>(IHandlerConfigurator\<TMessage\>, Int32, IReceiveEndpointConfigurator, String)**

Limits the number of concurrent messages consumed by the handler.

```csharp
public static void UseConcurrentMessageLimit<TMessage>(IHandlerConfigurator<TMessage> configurator, int concurrentMessageLimit, IReceiveEndpointConfigurator managementEndpointConfigurator, string id)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrent message limit for the handler message type

`managementEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
A management endpoint configurator to support runtime adjustment

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
An identifier for the concurrency limit to allow selective adjustment

### **UseConcurrentMessageLimit\<TMessage\>(IPipeConfigurator\<ConsumeContext\<TMessage\>\>, Int32)**

Limits the number of concurrent messages consumed for the specified message type.

```csharp
public static void UseConcurrentMessageLimit<TMessage>(IPipeConfigurator<ConsumeContext<TMessage>> configurator, int concurrentMessageLimit)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrent message limit for the message type

### **UseConcurrentMessageLimit\<TMessage\>(IPipeConfigurator\<ConsumeContext\<TMessage\>\>, Int32, IReceiveEndpointConfigurator, String)**

Limits the number of concurrent messages consumed for the specified message type.

```csharp
public static void UseConcurrentMessageLimit<TMessage>(IPipeConfigurator<ConsumeContext<TMessage>> configurator, int concurrentMessageLimit, IReceiveEndpointConfigurator managementEndpointConfigurator, string id)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrent message limit for the message type

`managementEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
A management endpoint configurator to support runtime adjustment

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
An identifier for the concurrency limit to allow selective adjustment
