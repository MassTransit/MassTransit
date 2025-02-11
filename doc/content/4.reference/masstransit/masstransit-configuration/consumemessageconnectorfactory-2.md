---

title: ConsumeMessageConnectorFactory<TConsumer, TMessage>

---

# ConsumeMessageConnectorFactory\<TConsumer, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class ConsumeMessageConnectorFactory<TConsumer, TMessage> : IMessageConnectorFactory
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeMessageConnectorFactory\<TConsumer, TMessage\>](../masstransit-configuration/consumemessageconnectorfactory-2)<br/>
Implements [IMessageConnectorFactory](../masstransit-configuration/imessageconnectorfactory)

## Constructors

### **ConsumeMessageConnectorFactory()**

```csharp
public ConsumeMessageConnectorFactory()
```

## Methods

### **CreateConsumerConnector\<T\>()**

```csharp
public IConsumerMessageConnector<T> CreateConsumerConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IConsumerMessageConnector\<T\>](../masstransit-configuration/iconsumermessageconnector-1)<br/>
