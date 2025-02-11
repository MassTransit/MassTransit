---

title: TestConsumerFactoryDecorator<TConsumer>

---

# TestConsumerFactoryDecorator\<TConsumer\>

Namespace: MassTransit.Testing.Implementations

```csharp
public class TestConsumerFactoryDecorator<TConsumer> : IConsumerFactory<TConsumer>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TestConsumerFactoryDecorator\<TConsumer\>](../masstransit-testing-implementations/testconsumerfactorydecorator-1)<br/>
Implements [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **TestConsumerFactoryDecorator(IConsumerFactory\<TConsumer\>, ReceivedMessageList)**

```csharp
public TestConsumerFactoryDecorator(IConsumerFactory<TConsumer> consumerFactory, ReceivedMessageList received)
```

#### Parameters

`consumerFactory` [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1)<br/>

`received` [ReceivedMessageList](../masstransit-testing/receivedmessagelist)<br/>

## Methods

### **Send\<TMessage\>(ConsumeContext\<TMessage\>, IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

```csharp
public Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
