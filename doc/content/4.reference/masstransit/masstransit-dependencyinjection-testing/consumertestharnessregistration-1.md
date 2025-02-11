---

title: ConsumerTestHarnessRegistration<TConsumer>

---

# ConsumerTestHarnessRegistration\<TConsumer\>

Namespace: MassTransit.DependencyInjection.Testing

```csharp
public class ConsumerTestHarnessRegistration<TConsumer> : IConsumerFactoryDecoratorRegistration<TConsumer>
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerTestHarnessRegistration\<TConsumer\>](../masstransit-dependencyinjection-testing/consumertestharnessregistration-1)<br/>
Implements [IConsumerFactoryDecoratorRegistration\<TConsumer\>](../masstransit-dependencyinjection-registration/iconsumerfactorydecoratorregistration-1)

## Properties

### **Consumed**

```csharp
public ReceivedMessageList Consumed { get; }
```

#### Property Value

[ReceivedMessageList](../masstransit-testing/receivedmessagelist)<br/>

## Constructors

### **ConsumerTestHarnessRegistration(BusTestHarness)**

```csharp
public ConsumerTestHarnessRegistration(BusTestHarness testHarness)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

## Methods

### **DecorateConsumerFactory(IConsumerFactory\<TConsumer\>)**

```csharp
public IConsumerFactory<TConsumer> DecorateConsumerFactory(IConsumerFactory<TConsumer> consumerFactory)
```

#### Parameters

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

#### Returns

[IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>
