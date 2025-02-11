---

title: IInstanceConnector

---

# IInstanceConnector

Namespace: MassTransit.Configuration

```csharp
public interface IInstanceConnector
```

## Methods

### **CreateConsumerSpecification\<TConsumer\>()**

```csharp
IConsumerSpecification<TConsumer> CreateConsumerSpecification<TConsumer>()
```

#### Type Parameters

`TConsumer`<br/>

#### Returns

[IConsumerSpecification\<TConsumer\>](../masstransit-configuration/iconsumerspecification-1)<br/>

### **ConnectInstance(IConsumePipeConnector, Object)**

```csharp
ConnectHandle ConnectInstance(IConsumePipeConnector pipeConnector, object instance)
```

#### Parameters

`pipeConnector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`instance` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectInstance\<TInstance\>(IConsumePipeConnector, TInstance, IConsumerSpecification\<TInstance\>)**

```csharp
ConnectHandle ConnectInstance<TInstance>(IConsumePipeConnector pipeConnector, TInstance instance, IConsumerSpecification<TInstance> specification)
```

#### Type Parameters

`TInstance`<br/>

#### Parameters

`pipeConnector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`instance` TInstance<br/>

`specification` [IConsumerSpecification\<TInstance\>](../masstransit-configuration/iconsumerspecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
