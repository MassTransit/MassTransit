---

title: IConsumerFactoryDecoratorRegistration<TConsumer>

---

# IConsumerFactoryDecoratorRegistration\<TConsumer\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public interface IConsumerFactoryDecoratorRegistration<TConsumer>
```

#### Type Parameters

`TConsumer`<br/>

## Properties

### **Consumed**

```csharp
public abstract ReceivedMessageList Consumed { get; }
```

#### Property Value

[ReceivedMessageList](../masstransit-testing/receivedmessagelist)<br/>

## Methods

### **DecorateConsumerFactory(IConsumerFactory\<TConsumer\>)**

Decorate the container-based consumer factory, returning the consumer factory that should be
 used for receive endpoint registration

```csharp
IConsumerFactory<TConsumer> DecorateConsumerFactory(IConsumerFactory<TConsumer> consumerFactory)
```

#### Parameters

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

#### Returns

[IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>
