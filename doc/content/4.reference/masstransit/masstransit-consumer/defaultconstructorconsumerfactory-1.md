---

title: DefaultConstructorConsumerFactory<TConsumer>

---

# DefaultConstructorConsumerFactory\<TConsumer\>

Namespace: MassTransit.Consumer

```csharp
public class DefaultConstructorConsumerFactory<TConsumer> : IConsumerFactory<TConsumer>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultConstructorConsumerFactory\<TConsumer\>](../masstransit-consumer/defaultconstructorconsumerfactory-1)<br/>
Implements [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DefaultConstructorConsumerFactory()**

```csharp
public DefaultConstructorConsumerFactory()
```

## Methods

### **Send\<T\>(ConsumeContext\<T\>, IPipe\<ConsumerConsumeContext\<TConsumer, T\>\>)**

```csharp
public Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<ConsumerConsumeContext\<TConsumer, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
