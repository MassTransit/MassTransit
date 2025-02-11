---

title: ConsumerTestHarnessExtensions

---

# ConsumerTestHarnessExtensions

Namespace: MassTransit.Testing

```csharp
public static class ConsumerTestHarnessExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerTestHarnessExtensions](../masstransit-testing/consumertestharnessextensions)

## Methods

### **Consumer\<T\>(BusTestHarness, String)**

```csharp
public static ConsumerTestHarness<T> Consumer<T>(BusTestHarness harness, string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ConsumerTestHarness\<T\>](../masstransit-testing/consumertestharness-1)<br/>

### **Consumer\<T\>(BusTestHarness, Action\<IConsumerConfigurator\<T\>\>, String)**

```csharp
public static ConsumerTestHarness<T> Consumer<T>(BusTestHarness harness, Action<IConsumerConfigurator<T>> configure, string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ConsumerTestHarness\<T\>](../masstransit-testing/consumertestharness-1)<br/>

### **Consumer\<T\>(BusTestHarness, IConsumerFactory\<T\>, String)**

```csharp
public static ConsumerTestHarness<T> Consumer<T>(BusTestHarness harness, IConsumerFactory<T> consumerFactory, string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`consumerFactory` [IConsumerFactory\<T\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ConsumerTestHarness\<T\>](../masstransit-testing/consumertestharness-1)<br/>

### **Consumer\<T\>(BusTestHarness, IConsumerFactory\<T\>, Action\<IConsumerConfigurator\<T\>\>, String)**

```csharp
public static ConsumerTestHarness<T> Consumer<T>(BusTestHarness harness, IConsumerFactory<T> consumerFactory, Action<IConsumerConfigurator<T>> configure, string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`consumerFactory` [IConsumerFactory\<T\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ConsumerTestHarness\<T\>](../masstransit-testing/consumertestharness-1)<br/>

### **Consumer\<T\>(BusTestHarness, Func\<T\>, String)**

```csharp
public static ConsumerTestHarness<T> Consumer<T>(BusTestHarness harness, Func<T> consumerFactoryMethod, string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`consumerFactoryMethod` [Func\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ConsumerTestHarness\<T\>](../masstransit-testing/consumertestharness-1)<br/>

### **Consumer\<T\>(BusTestHarness, Func\<T\>, Action\<IConsumerConfigurator\<T\>\>, String)**

```csharp
public static ConsumerTestHarness<T> Consumer<T>(BusTestHarness harness, Func<T> consumerFactoryMethod, Action<IConsumerConfigurator<T>> configure, string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`consumerFactoryMethod` [Func\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ConsumerTestHarness\<T\>](../masstransit-testing/consumertestharness-1)<br/>
