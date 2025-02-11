---

title: ISagaConfigurationObserver

---

# ISagaConfigurationObserver

Namespace: MassTransit

```csharp
public interface ISagaConfigurationObserver
```

## Methods

### **SagaConfigured\<TSaga\>(ISagaConfigurator\<TSaga\>)**

Called immediately after the saga configuration is completed, but before the saga pipeline is built.

```csharp
void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../masstransit/isagaconfigurator-1)<br/>

### **StateMachineSagaConfigured\<TInstance\>(ISagaConfigurator\<TInstance\>, SagaStateMachine\<TInstance\>)**

Called immediately after the state machine saga configuration is completed, but before the saga pipeline is built. Note that
 [ISagaConfigurationObserver.SagaConfigured\<TSaga\>(ISagaConfigurator\<TSaga\>)](isagaconfigurationobserver#sagaconfiguredtsagaisagaconfiguratortsaga) method will also be called, for backwards compatibility

```csharp
void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
```

#### Type Parameters

`TInstance`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TInstance\>](../masstransit/isagaconfigurator-1)<br/>

`stateMachine` [SagaStateMachine\<TInstance\>](../masstransit/sagastatemachine-1)<br/>

### **SagaMessageConfigured\<TSaga, TMessage\>(ISagaMessageConfigurator\<TSaga, TMessage\>)**

Called after the saga/message configuration is completed, but before the saga/message pipeline is built.

```csharp
void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [ISagaMessageConfigurator\<TSaga, TMessage\>](../masstransit/isagamessageconfigurator-2)<br/>
