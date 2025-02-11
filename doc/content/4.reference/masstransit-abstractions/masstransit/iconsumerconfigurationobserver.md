---

title: IConsumerConfigurationObserver

---

# IConsumerConfigurationObserver

Namespace: MassTransit

```csharp
public interface IConsumerConfigurationObserver
```

## Methods

### **ConsumerConfigured\<TConsumer\>(IConsumerConfigurator\<TConsumer\>)**

Called when a consumer is configured

```csharp
void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../masstransit/iconsumerconfigurator-1)<br/>

### **ConsumerMessageConfigured\<TConsumer, TMessage\>(IConsumerMessageConfigurator\<TConsumer, TMessage\>)**

Called when a consumer/message combination is configured

```csharp
void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IConsumerMessageConfigurator\<TConsumer, TMessage\>](../masstransit/iconsumermessageconfigurator-2)<br/>
