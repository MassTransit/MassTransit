---

title: ISagaSpecification<TSaga>

---

# ISagaSpecification\<TSaga\>

Namespace: MassTransit.Configuration

A consumer specification, that can be modified

```csharp
public interface ISagaSpecification<TSaga> : ISagaConfigurator<TSaga>, IPipeConfigurator<SagaConsumeContext<TSaga>>, ISagaConfigurationObserverConnector, IConsumeConfigurator, IOptionsSet, ISpecification
```

#### Type Parameters

`TSaga`<br/>

Implements [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1), [IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IOptionsSet](../../masstransit-abstractions/masstransit-configuration/ioptionsset), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Methods

### **GetMessageSpecification\<T\>()**

```csharp
ISagaMessageSpecification<TSaga, T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaMessageSpecification\<TSaga, T\>](../masstransit-configuration/isagamessagespecification-2)<br/>

### **ConfigureMessagePipe\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>)**

Apply any saga-wide configurations to the message pipe, such as concurrency limit, etc.

```csharp
void ConfigureMessagePipe<T>(IPipeConfigurator<ConsumeContext<T>> pipeConfigurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipeConfigurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
