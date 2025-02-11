---

title: RetryConfigurationExtensions

---

# RetryConfigurationExtensions

Namespace: MassTransit

```csharp
public static class RetryConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RetryConfigurationExtensions](../masstransit/retryconfigurationextensions)

## Methods

### **UseRetry(IPipeConfigurator\<ConsumeContext\>, Action\<IRetryConfigurator\>)**

#### Caution

Use UseMessageRetry instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void UseRetry(IPipeConfigurator<ConsumeContext> configurator, Action<IRetryConfigurator> configure)
```

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRetry\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, Action\<IRetryConfigurator\>)**

#### Caution

Use UseMessageRetry instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void UseRetry<T>(IPipeConfigurator<ConsumeContext<T>> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseMessageRetry\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, Action\<IRetryConfigurator\>)**

```csharp
public static void UseMessageRetry<T>(IPipeConfigurator<ConsumeContext<T>> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRetry\<T\>(IConsumePipeConfigurator, Action\<IRetryConfigurator\>)**

#### Caution

Use UseMessageRetry instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void UseRetry<T>(IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRetry\<TConsumer\>(IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>, Action\<IRetryConfigurator\>)**

#### Caution

Use UseMessageRetry instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void UseRetry<TConsumer>(IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRetry\<TSaga\>(IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>, Action\<IRetryConfigurator\>)**

#### Caution

Use UseMessageRetry instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void UseRetry<TSaga>(IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRetry\<T\>(IPipeConfigurator\<T\>, Action\<IRetryConfigurator\>)**

```csharp
public static void UseRetry<T>(IPipeConfigurator<T> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRetry(IPipeConfigurator\<ConsumeContext\>, IBusFactoryConfigurator, Action\<IRetryConfigurator\>)**

#### Caution

Use UseMessageRetry instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void UseRetry(IPipeConfigurator<ConsumeContext> configurator, IBusFactoryConfigurator connector, Action<IRetryConfigurator> configure)
```

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`connector` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRetry(IBusFactoryConfigurator, Action\<IRetryConfigurator\>)**

#### Caution

Use UseMessageRetry instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void UseRetry(IBusFactoryConfigurator configurator, Action<IRetryConfigurator> configure)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRetry\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, IBusFactoryConfigurator, Action\<IRetryConfigurator\>)**

#### Caution

Use UseMessageRetry instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void UseRetry<T>(IPipeConfigurator<ConsumeContext<T>> configurator, IBusFactoryConfigurator connector, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`connector` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRetry\<TConsumer\>(IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>, IBusFactoryConfigurator, Action\<IRetryConfigurator\>)**

#### Caution

Use UseMessageRetry instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void UseRetry<TConsumer>(IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, IBusFactoryConfigurator connector, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`connector` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseRetry\<TSaga\>(IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>, IBusFactoryConfigurator, Action\<IRetryConfigurator\>)**

#### Caution

Use UseMessageRetry instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void UseRetry<TSaga>(IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, IBusFactoryConfigurator connector, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`connector` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **None(IRetryConfigurator)**

Create a policy that does not retry any messages.

```csharp
public static IRetryConfigurator None(IRetryConfigurator configurator)
```

#### Parameters

`configurator` [IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

#### Returns

[IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

### **Immediate(IRetryConfigurator, Int32)**

Create an immediate retry policy with the specified number of retries, with no
 delay between attempts.

```csharp
public static IRetryConfigurator Immediate(IRetryConfigurator configurator, int retryLimit)
```

#### Parameters

`configurator` [IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retries to attempt

#### Returns

[IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

### **Intervals(IRetryConfigurator, TimeSpan[])**

Create an interval retry policy with the specified intervals. The retry count equals
 the number of intervals provided

```csharp
public static IRetryConfigurator Intervals(IRetryConfigurator configurator, TimeSpan[] intervals)
```

#### Parameters

`configurator` [IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

`intervals` [TimeSpan[]](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The intervals before each subsequent retry attempt

#### Returns

[IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

### **Intervals(IRetryConfigurator, Int32[])**

Create an interval retry policy with the specified intervals. The retry count equals
 the number of intervals provided

```csharp
public static IRetryConfigurator Intervals(IRetryConfigurator configurator, Int32[] intervals)
```

#### Parameters

`configurator` [IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

`intervals` [Int32[]](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The intervals in milliseconds before each subsequent retry attempt

#### Returns

[IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

### **Interval(IRetryConfigurator, Int32, TimeSpan)**

Create an interval retry policy with the specified number of retries at a fixed interval

```csharp
public static IRetryConfigurator Interval(IRetryConfigurator configurator, int retryCount, TimeSpan interval)
```

#### Parameters

`configurator` [IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

`retryCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retry attempts

`interval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The interval between each retry attempt

#### Returns

[IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

### **Interval(IRetryConfigurator, Int32, Int32)**

Create an interval retry policy with the specified number of retries at a fixed interval

```csharp
public static IRetryConfigurator Interval(IRetryConfigurator configurator, int retryCount, int interval)
```

#### Parameters

`configurator` [IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

`retryCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retry attempts

`interval` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The interval in milliseconds between each retry attempt

#### Returns

[IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

### **Exponential(IRetryConfigurator, Int32, TimeSpan, TimeSpan, TimeSpan)**

Create an exponential retry policy with the specified number of retries at exponential
 intervals

```csharp
public static IRetryConfigurator Exponential(IRetryConfigurator configurator, int retryLimit, TimeSpan minInterval, TimeSpan maxInterval, TimeSpan intervalDelta)
```

#### Parameters

`configurator` [IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`maxInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`intervalDelta` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

### **Incremental(IRetryConfigurator, Int32, TimeSpan, TimeSpan)**

Create an incremental retry policy with the specified number of retry attempts with an incrementing
 interval between retries

```csharp
public static IRetryConfigurator Incremental(IRetryConfigurator configurator, int retryLimit, TimeSpan initialInterval, TimeSpan intervalIncrement)
```

#### Parameters

`configurator` [IRetryConfigurator](../masstransit/iretryconfigurator)<br/>

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retry attempts

`initialInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The initial retry interval

`intervalIncrement` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The interval to add to the retry interval with each subsequent retry

#### Returns

[IRetryConfigurator](../masstransit/iretryconfigurator)<br/>
