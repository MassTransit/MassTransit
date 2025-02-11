---

title: IConsumerFactory<TConsumer>

---

# IConsumerFactory\<TConsumer\>

Namespace: MassTransit

Maps an instance of a consumer to one or more Consume methods for the specified message type
 The whole purpose for this interface is to allow the creator of the consumer to manage the lifecycle
 of the consumer, along with anything else that needs to be managed by the factory, container, etc.

```csharp
public interface IConsumerFactory<TConsumer> : IProbeSite
```

#### Type Parameters

`TConsumer`<br/>
The Consumer type

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Send\<T\>(ConsumeContext\<T\>, IPipe\<ConsumerConsumeContext\<TConsumer, T\>\>)**

```csharp
Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

`next` [IPipe\<ConsumerConsumeContext\<TConsumer, T\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
