---

title: IInstanceMessageConnector<TInstance>

---

# IInstanceMessageConnector\<TInstance\>

Namespace: MassTransit.Configuration

```csharp
public interface IInstanceMessageConnector<TInstance> : IInstanceMessageConnector
```

#### Type Parameters

`TInstance`<br/>

Implements [IInstanceMessageConnector](../masstransit-configuration/iinstancemessageconnector)

## Methods

### **CreateConsumerMessageSpecification()**

```csharp
IConsumerMessageSpecification<TInstance> CreateConsumerMessageSpecification()
```

#### Returns

[IConsumerMessageSpecification\<TInstance\>](../masstransit-configuration/iconsumermessagespecification-1)<br/>

### **ConnectInstance(IConsumePipeConnector, TInstance, IConsumerSpecification\<TInstance\>)**

```csharp
ConnectHandle ConnectInstance(IConsumePipeConnector pipeConnector, TInstance instance, IConsumerSpecification<TInstance> specification)
```

#### Parameters

`pipeConnector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`instance` TInstance<br/>

`specification` [IConsumerSpecification\<TInstance\>](../masstransit-configuration/iconsumerspecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
