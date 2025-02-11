---

title: ConsumerContainerTestHarnessRegistration<TConsumer>

---

# ConsumerContainerTestHarnessRegistration\<TConsumer\>

Namespace: MassTransit.DependencyInjection.Testing

```csharp
public class ConsumerContainerTestHarnessRegistration<TConsumer> : IConsumerFactoryDecoratorRegistration<TConsumer>
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerContainerTestHarnessRegistration\<TConsumer\>](../masstransit-dependencyinjection-testing/consumercontainertestharnessregistration-1)<br/>
Implements [IConsumerFactoryDecoratorRegistration\<TConsumer\>](../masstransit-dependencyinjection-registration/iconsumerfactorydecoratorregistration-1)

## Properties

### **Consumed**

```csharp
public ReceivedMessageList Consumed { get; }
```

#### Property Value

[ReceivedMessageList](../masstransit-testing/receivedmessagelist)<br/>

## Constructors

### **ConsumerContainerTestHarnessRegistration(ITestHarness)**

```csharp
public ConsumerContainerTestHarnessRegistration(ITestHarness testHarness)
```

#### Parameters

`testHarness` [ITestHarness](../masstransit-testing/itestharness)<br/>

## Methods

### **DecorateConsumerFactory(IConsumerFactory\<TConsumer\>)**

```csharp
public IConsumerFactory<TConsumer> DecorateConsumerFactory(IConsumerFactory<TConsumer> consumerFactory)
```

#### Parameters

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

#### Returns

[IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>
