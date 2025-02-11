---

title: IBatchConfigurator<TMessage>

---

# IBatchConfigurator\<TMessage\>

Namespace: MassTransit

Batching is an experimental feature, and may be changed at any time in the future.

```csharp
public interface IBatchConfigurator<TMessage> : IConsumeConfigurator
```

#### Type Parameters

`TMessage`<br/>

Implements [IConsumeConfigurator](../masstransit/iconsumeconfigurator)

## Properties

### **TimeLimit**

Set the maximum time to wait for messages before the batch is automatically completed

```csharp
public abstract TimeSpan TimeLimit { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TimeLimitStart**

Sets the starting point for the [IBatchConfigurator\<TMessage\>.TimeLimit](ibatchconfigurator-1#timelimit)

```csharp
public abstract BatchTimeLimitStart TimeLimitStart { set; }
```

#### Property Value

[BatchTimeLimitStart](../masstransit/batchtimelimitstart)<br/>

### **MessageLimit**

Set the maximum number of messages which can be added to a single batch

```csharp
public abstract int MessageLimit { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrencyLimit**

Set the maximum number of concurrent batches which can execute at the same time

```csharp
public abstract int ConcurrencyLimit { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Consumer\<TConsumer\>(IConsumerFactory\<TConsumer\>, Action\<IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>\>)**

Specify the consumer factory for the batch message consumer

```csharp
void Consumer<TConsumer>(IConsumerFactory<TConsumer> consumerFactory, Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`consumerFactory` [IConsumerFactory\<TConsumer\>](../masstransit/iconsumerfactory-1)<br/>

`configure` [Action\<IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the consumer pipe
