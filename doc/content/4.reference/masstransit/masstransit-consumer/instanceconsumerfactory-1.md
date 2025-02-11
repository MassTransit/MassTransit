---

title: InstanceConsumerFactory<TConsumer>

---

# InstanceConsumerFactory\<TConsumer\>

Namespace: MassTransit.Consumer

Retains a reference to an existing message consumer, and uses it to send consumable messages for
 processing.

```csharp
public class InstanceConsumerFactory<TConsumer> : IConsumerFactory<TConsumer>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstanceConsumerFactory\<TConsumer\>](../masstransit-consumer/instanceconsumerfactory-1)<br/>
Implements [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **InstanceConsumerFactory(TConsumer)**

```csharp
public InstanceConsumerFactory(TConsumer consumer)
```

#### Parameters

`consumer` TConsumer<br/>

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
