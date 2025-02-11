---

title: ScopeConsumerFactory<TConsumer>

---

# ScopeConsumerFactory\<TConsumer\>

Namespace: MassTransit.DependencyInjection

```csharp
public class ScopeConsumerFactory<TConsumer> : IConsumerFactory<TConsumer>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopeConsumerFactory\<TConsumer\>](../masstransit-dependencyinjection/scopeconsumerfactory-1)<br/>
Implements [IConsumerFactory\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScopeConsumerFactory(IConsumeScopeProvider)**

```csharp
public ScopeConsumerFactory(IConsumeScopeProvider scopeProvider)
```

#### Parameters

`scopeProvider` [IConsumeScopeProvider](../masstransit-dependencyinjection/iconsumescopeprovider)<br/>

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
