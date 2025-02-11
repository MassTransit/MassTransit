---

title: InstanceConnector<TConsumer>

---

# InstanceConnector\<TConsumer\>

Namespace: MassTransit.Configuration

```csharp
public class InstanceConnector<TConsumer> : IInstanceConnector
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstanceConnector\<TConsumer\>](../masstransit-configuration/instanceconnector-1)<br/>
Implements [IInstanceConnector](../masstransit-configuration/iinstanceconnector)

## Constructors

### **InstanceConnector()**

```csharp
public InstanceConnector()
```

## Methods

### **ConnectInstance\<T\>(IConsumePipeConnector, T, IConsumerSpecification\<T\>)**

```csharp
public ConnectHandle ConnectInstance<T>(IConsumePipeConnector pipeConnector, T instance, IConsumerSpecification<T> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipeConnector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`instance` T<br/>

`specification` [IConsumerSpecification\<T\>](../masstransit-configuration/iconsumerspecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectInstance(IConsumePipeConnector, Object)**

```csharp
public ConnectHandle ConnectInstance(IConsumePipeConnector pipeConnector, object instance)
```

#### Parameters

`pipeConnector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`instance` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **CreateConsumerSpecification\<T\>()**

```csharp
public IConsumerSpecification<T> CreateConsumerSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IConsumerSpecification\<T\>](../masstransit-configuration/iconsumerspecification-1)<br/>
