---

title: ISagaConfigurator<TSaga>

---

# ISagaConfigurator\<TSaga\>

Namespace: MassTransit

```csharp
public interface ISagaConfigurator<TSaga> : IPipeConfigurator<SagaConsumeContext<TSaga>>, ISagaConfigurationObserverConnector, IConsumeConfigurator, IOptionsSet
```

#### Type Parameters

`TSaga`<br/>

Implements [IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>](../masstransit/ipipeconfigurator-1), [ISagaConfigurationObserverConnector](../masstransit/isagaconfigurationobserverconnector), [IConsumeConfigurator](../masstransit/iconsumeconfigurator), [IOptionsSet](../masstransit-configuration/ioptionsset)

## Properties

### **ConcurrentMessageLimit**

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **Message\<T\>(Action\<ISagaMessageConfigurator\<T\>\>)**

Add middleware to the message pipeline, which is invoked prior to the saga repository.

```csharp
void Message<T>(Action<ISagaMessageConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configure` [Action\<ISagaMessageConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback to configure the message pipeline

### **SagaMessage\<T\>(Action\<ISagaMessageConfigurator\<TSaga, T\>\>)**

Add middleware to the saga pipeline, for the specified message type, which is invoked
 after the saga repository.

```csharp
void SagaMessage<T>(Action<ISagaMessageConfigurator<TSaga, T>> configure)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configure` [Action\<ISagaMessageConfigurator\<TSaga, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback to configure the message pipeline
