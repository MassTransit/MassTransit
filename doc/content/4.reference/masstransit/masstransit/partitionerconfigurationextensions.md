---

title: PartitionerConfigurationExtensions

---

# PartitionerConfigurationExtensions

Namespace: MassTransit

```csharp
public static class PartitionerConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PartitionerConfigurationExtensions](../masstransit/partitionerconfigurationextensions)

## Methods

### **UseMessagePartitioner(IConsumePipeConfigurator, Int32)**

Adds partitioning to the consume pipeline, with a number of partitions handling all message types on the receive endpoint. Endpoints must have
 a CorrelationId provider available, which can be specified using GlobalTopology.Send.UseCorrelationId&lt;T&gt;(x =&gt; x.SomeId);

```csharp
public static void UseMessagePartitioner(IConsumePipeConfigurator configurator, int partitionCount)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>
The pipe configurator

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions

### **UsePartitioner\<T\>(IConsumePipeConfigurator, IPartitioner, Func\<ConsumeContext\<T\>, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IConsumePipeConfigurator configurator, IPartitioner partitioner, Func<ConsumeContext<T>, Guid> keyProvider)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
An existing partitioner that is shared

`keyProvider` [Func\<ConsumeContext\<T\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, IPartitioner, Func\<ConsumeContext\<T\>, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IPipeConfigurator<ConsumeContext<T>> configurator, IPartitioner partitioner, Func<ConsumeContext<T>, Guid> keyProvider)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
An existing partitioner that is shared

`keyProvider` [Func\<ConsumeContext\<T\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, Int32, Func\<ConsumeContext\<T\>, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IPipeConfigurator<ConsumeContext<T>> configurator, int partitionCount, Func<ConsumeContext<T>, Guid> keyProvider)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<ConsumeContext\<T\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<TConsumer\>(IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>, Int32, Func\<ConsumerConsumeContext\<TConsumer\>, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TConsumer>(IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, int partitionCount, Func<ConsumerConsumeContext<TConsumer>, Guid> keyProvider)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<ConsumerConsumeContext\<TConsumer\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<TConsumer\>(IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>, Int32, Func\<ConsumerConsumeContext\<TConsumer\>, String\>, Encoding)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TConsumer>(IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, int partitionCount, Func<ConsumerConsumeContext<TConsumer>, string> keyProvider, Encoding encoding)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<ConsumerConsumeContext\<TConsumer\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br/>

### **UsePartitioner\<TSaga\>(IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>, Int32, Func\<SagaConsumeContext\<TSaga\>, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TSaga>(IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, int partitionCount, Func<SagaConsumeContext<TSaga>, Guid> keyProvider)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<SagaConsumeContext\<TSaga\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<TSaga\>(IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>, Int32, Func\<SagaConsumeContext\<TSaga\>, String\>, Encoding)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TSaga>(IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, int partitionCount, Func<SagaConsumeContext<TSaga>, string> keyProvider, Encoding encoding)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<SagaConsumeContext\<TSaga\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br/>

### **UsePartitioner\<TActivity, TArguments\>(IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>, Int32, Func\<ExecuteActivityContext\<TActivity, TArguments\>, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TActivity, TArguments>(IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator, int partitionCount, Func<ExecuteActivityContext<TActivity, TArguments>, Guid> keyProvider)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<ExecuteActivityContext\<TActivity, TArguments\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<TActivity, TArguments\>(IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>, IPartitioner, Func\<ExecuteActivityContext\<TActivity, TArguments\>, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TActivity, TArguments>(IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator, IPartitioner partitioner, Func<ExecuteActivityContext<TActivity, TArguments>, Guid> keyProvider)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
An existing partitioner to share

`keyProvider` [Func\<ExecuteActivityContext\<TActivity, TArguments\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<TActivity, TArguments\>(IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>, Int32, Func\<ExecuteActivityContext\<TActivity, TArguments\>, String\>, Encoding)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TActivity, TArguments>(IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator, int partitionCount, Func<ExecuteActivityContext<TActivity, TArguments>, string> keyProvider, Encoding encoding)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<ExecuteActivityContext\<TActivity, TArguments\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br/>
The text encoding to use to convert the string to byte[] (defaults to UTF8)

### **UsePartitioner\<TActivity, TArguments\>(IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>, IPartitioner, Func\<ExecuteActivityContext\<TActivity, TArguments\>, String\>, Encoding)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TActivity, TArguments>(IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator, IPartitioner partitioner, Func<ExecuteActivityContext<TActivity, TArguments>, string> keyProvider, Encoding encoding)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
An existing partitioner to share

`keyProvider` [Func\<ExecuteActivityContext\<TActivity, TArguments\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br/>
The text encoding to use to convert the string to byte[] (defaults to UTF8)

### **UsePartitioner\<TActivity, TLog\>(IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>, Int32, Func\<CompensateActivityContext\<TActivity, TLog\>, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TActivity, TLog>(IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator, int partitionCount, Func<CompensateActivityContext<TActivity, TLog>, Guid> keyProvider)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<CompensateActivityContext\<TActivity, TLog\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<TActivity, TLog\>(IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>, IPartitioner, Func\<CompensateActivityContext\<TActivity, TLog\>, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TActivity, TLog>(IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator, IPartitioner partitioner, Func<CompensateActivityContext<TActivity, TLog>, Guid> keyProvider)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
An existing partitioner to share

`keyProvider` [Func\<CompensateActivityContext\<TActivity, TLog\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<TActivity, TLog\>(IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>, Int32, Func\<CompensateActivityContext\<TActivity, TLog\>, String\>, Encoding)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TActivity, TLog>(IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator, int partitionCount, Func<CompensateActivityContext<TActivity, TLog>, string> keyProvider, Encoding encoding)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<CompensateActivityContext\<TActivity, TLog\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br/>
The text encoding to use to convert the string to byte[] (defaults to UTF8)

### **UsePartitioner\<TActivity, TLog\>(IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>, IPartitioner, Func\<CompensateActivityContext\<TActivity, TLog\>, String\>, Encoding)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<TActivity, TLog>(IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator, IPartitioner partitioner, Func<CompensateActivityContext<TActivity, TLog>, string> keyProvider, Encoding encoding)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
An existing partitioner to share

`keyProvider` [Func\<CompensateActivityContext\<TActivity, TLog\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br/>
The text encoding to use to convert the string to byte[] (defaults to UTF8)

### **CreatePartitioner\<T\>(IPipeConfigurator\<T\>, Int32)**

Create a partitioner which can be used across multiple partitioner filters

```csharp
public static IPartitioner CreatePartitioner<T>(IPipeConfigurator<T> _, int partitionCount)
```

#### Type Parameters

`T`<br/>

#### Parameters

`_` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[IPartitioner](../masstransit/ipartitioner)<br/>

### **UsePartitioner\<T\>(IPipeConfigurator\<T\>, Int32, Func\<T, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IPipeConfigurator<T> configurator, int partitionCount, Func<T, Guid> keyProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<T, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<T\>(IPipeConfigurator\<T\>, IPartitioner, Func\<T, Guid\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IPipeConfigurator<T> configurator, IPartitioner partitioner, Func<T, Guid> keyProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
An existing partitioner that is shared

`keyProvider` [Func\<T, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<T\>(IPipeConfigurator\<T\>, Int32, Func\<T, String\>, Encoding)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IPipeConfigurator<T> configurator, int partitionCount, Func<T, string> keyProvider, Encoding encoding)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<T, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br/>

### **UsePartitioner\<T\>(IPipeConfigurator\<T\>, IPartitioner, Func\<T, String\>, Encoding)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IPipeConfigurator<T> configurator, IPartitioner partitioner, Func<T, string> keyProvider, Encoding encoding)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
An existing partitioner that is shared

`keyProvider` [Func\<T, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br/>

### **UsePartitioner\<T\>(IPipeConfigurator\<T\>, Int32, Func\<T, Int64\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IPipeConfigurator<T> configurator, int partitionCount, Func<T, long> keyProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<T, Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<T\>(IPipeConfigurator\<T\>, IPartitioner, Func\<T, Int64\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IPipeConfigurator<T> configurator, IPartitioner partitioner, Func<T, long> keyProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
An existing partitioner that is shared

`keyProvider` [Func\<T, Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<T\>(IPipeConfigurator\<T\>, Int32, Func\<T, Byte[]\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IPipeConfigurator<T> configurator, int partitionCount, Func<T, Byte[]> keyProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of partitions to use when distributing message delivery

`keyProvider` [Func\<T, Byte[]\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message

### **UsePartitioner\<T\>(IPipeConfigurator\<T\>, IPartitioner, Func\<T, Byte[]\>)**

Adds a partition filter, which also limits concurrency by the partition count.

```csharp
public static void UsePartitioner<T>(IPipeConfigurator<T> configurator, IPartitioner partitioner, Func<T, Byte[]> keyProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>
An existing partitioner that is shared

`keyProvider` [Func\<T, Byte[]\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Provides the key from the message
