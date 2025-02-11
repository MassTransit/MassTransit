---

title: SagaConfigurationObservable

---

# SagaConfigurationObservable

Namespace: MassTransit.Configuration

```csharp
public class SagaConfigurationObservable : Connectable<ISagaConfigurationObserver>, ISagaConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<ISagaConfigurationObserver\>](../masstransit-util/connectable-1) → [SagaConfigurationObservable](../masstransit-configuration/sagaconfigurationobservable)<br/>
Implements [ISagaConfigurationObserver](../masstransit/isagaconfigurationobserver)

## Properties

### **Connected**

```csharp
public ISagaConfigurationObserver[] Connected { get; }
```

#### Property Value

[ISagaConfigurationObserver[]](../masstransit/isagaconfigurationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **SagaConfigurationObservable()**

```csharp
public SagaConfigurationObservable()
```

## Methods

### **SagaConfigured\<TSaga\>(ISagaConfigurator\<TSaga\>)**

```csharp
public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../masstransit/isagaconfigurator-1)<br/>

### **StateMachineSagaConfigured\<TInstance\>(ISagaConfigurator\<TInstance\>, SagaStateMachine\<TInstance\>)**

```csharp
public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
```

#### Type Parameters

`TInstance`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TInstance\>](../masstransit/isagaconfigurator-1)<br/>

`stateMachine` [SagaStateMachine\<TInstance\>](../masstransit/sagastatemachine-1)<br/>

### **SagaMessageConfigured\<TSaga, TMessage\>(ISagaMessageConfigurator\<TSaga, TMessage\>)**

```csharp
public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [ISagaMessageConfigurator\<TSaga, TMessage\>](../masstransit/isagamessageconfigurator-2)<br/>

### **Method4()**

```csharp
public void Method4()
```

### **Method5()**

```csharp
public void Method5()
```

### **Method6()**

```csharp
public void Method6()
```
