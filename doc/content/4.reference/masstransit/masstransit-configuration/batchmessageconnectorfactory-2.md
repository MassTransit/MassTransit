---

title: BatchMessageConnectorFactory<TConsumer, TMessage>

---

# BatchMessageConnectorFactory\<TConsumer, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class BatchMessageConnectorFactory<TConsumer, TMessage> : IMessageConnectorFactory
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchMessageConnectorFactory\<TConsumer, TMessage\>](../masstransit-configuration/batchmessageconnectorfactory-2)<br/>
Implements [IMessageConnectorFactory](../masstransit-configuration/imessageconnectorfactory)

## Constructors

### **BatchMessageConnectorFactory()**

```csharp
public BatchMessageConnectorFactory()
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

### **CreateInstanceConnector\<T\>()**

```csharp
public IInstanceMessageConnector<T> CreateInstanceConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IInstanceMessageConnector\<T\>](../masstransit-configuration/iinstancemessageconnector-1)<br/>
