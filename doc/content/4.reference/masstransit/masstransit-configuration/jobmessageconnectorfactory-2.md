---

title: JobMessageConnectorFactory<TConsumer, TJob>

---

# JobMessageConnectorFactory\<TConsumer, TJob\>

Namespace: MassTransit.Configuration

```csharp
public class JobMessageConnectorFactory<TConsumer, TJob> : IMessageConnectorFactory
```

#### Type Parameters

`TConsumer`<br/>

`TJob`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobMessageConnectorFactory\<TConsumer, TJob\>](../masstransit-configuration/jobmessageconnectorfactory-2)<br/>
Implements [IMessageConnectorFactory](../masstransit-configuration/imessageconnectorfactory)

## Constructors

### **JobMessageConnectorFactory()**

```csharp
public JobMessageConnectorFactory()
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
