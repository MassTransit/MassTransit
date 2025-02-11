---

title: BatchConfigurator<TMessage>

---

# BatchConfigurator\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class BatchConfigurator<TMessage> : IBatchConfigurator<TMessage>, IConsumeConfigurator
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchConfigurator\<TMessage\>](../masstransit-configuration/batchconfigurator-1)<br/>
Implements [IBatchConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ibatchconfigurator-1), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator)

## Properties

### **TimeLimit**

```csharp
public TimeSpan TimeLimit { private get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TimeLimitStart**

```csharp
public BatchTimeLimitStart TimeLimitStart { private get; set; }
```

#### Property Value

[BatchTimeLimitStart](../../masstransit-abstractions/masstransit/batchtimelimitstart)<br/>

### **MessageLimit**

```csharp
public int MessageLimit { private get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrencyLimit**

```csharp
public int ConcurrencyLimit { private get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **BatchConfigurator(IReceiveEndpointConfigurator)**

```csharp
public BatchConfigurator(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

## Methods

### **Consumer\<TConsumer\>(IConsumerFactory\<TConsumer\>, Action\<IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>\>)**

```csharp
public void Consumer<TConsumer>(IConsumerFactory<TConsumer> consumerFactory, Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`configure` [Action\<IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
