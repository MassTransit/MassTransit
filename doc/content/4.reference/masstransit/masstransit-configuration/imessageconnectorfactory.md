---

title: IMessageConnectorFactory

---

# IMessageConnectorFactory

Namespace: MassTransit.Configuration

```csharp
public interface IMessageConnectorFactory
```

## Methods

### **CreateConsumerConnector\<T\>()**

```csharp
IConsumerMessageConnector<T> CreateConsumerConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IConsumerMessageConnector\<T\>](../masstransit-configuration/iconsumermessageconnector-1)<br/>

### **CreateInstanceConnector\<T\>()**

```csharp
IInstanceMessageConnector<T> CreateInstanceConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IInstanceMessageConnector\<T\>](../masstransit-configuration/iinstancemessageconnector-1)<br/>
