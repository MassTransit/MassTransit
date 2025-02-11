---

title: ScopedConsumePipeSpecificationObserver

---

# ScopedConsumePipeSpecificationObserver

Namespace: MassTransit.Configuration

```csharp
public class ScopedConsumePipeSpecificationObserver : IConsumerConfigurationObserver, ISagaConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedConsumePipeSpecificationObserver](../masstransit-configuration/scopedconsumepipespecificationobserver)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver)

## Constructors

### **ScopedConsumePipeSpecificationObserver(Type, IRegistrationContext, CompositeFilter\<Type\>)**

```csharp
public ScopedConsumePipeSpecificationObserver(Type filterType, IRegistrationContext context, CompositeFilter<Type> messageTypeFilter)
```

#### Parameters

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`messageTypeFilter` [CompositeFilter\<Type\>](../masstransit-configuration/compositefilter-1)<br/>

## Methods

### **ConsumerConfigured\<TConsumer\>(IConsumerConfigurator\<TConsumer\>)**

```csharp
public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

### **ConsumerMessageConfigured\<TConsumer, TMessage\>(IConsumerMessageConfigurator\<TConsumer, TMessage\>)**

```csharp
public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IConsumerMessageConfigurator\<TConsumer, TMessage\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2)<br/>

### **SagaConfigured\<TSaga\>(ISagaConfigurator\<TSaga\>)**

```csharp
public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

### **StateMachineSagaConfigured\<TInstance\>(ISagaConfigurator\<TInstance\>, SagaStateMachine\<TInstance\>)**

```csharp
public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
```

#### Type Parameters

`TInstance`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TInstance\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`stateMachine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

### **SagaMessageConfigured\<TSaga, TMessage\>(ISagaMessageConfigurator\<TSaga, TMessage\>)**

```csharp
public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [ISagaMessageConfigurator\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagamessageconfigurator-2)<br/>
