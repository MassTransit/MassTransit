---

title: ReceiveEndpointBuilder

---

# ReceiveEndpointBuilder

Namespace: MassTransit.Configuration

```csharp
public class ReceiveEndpointBuilder : IReceiveEndpointBuilder, IConsumePipeConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointBuilder](../masstransit-configuration/receiveendpointbuilder)<br/>
Implements [IReceiveEndpointBuilder](../../masstransit-abstractions/masstransit-configuration/ireceiveendpointbuilder), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)

## Constructors

### **ReceiveEndpointBuilder(IReceiveEndpointConfiguration)**

```csharp
public ReceiveEndpointBuilder(IReceiveEndpointConfiguration configuration)
```

#### Parameters

`configuration` [IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>

## Methods

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>)**

```csharp
public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>, ConnectPipeOptions)**

```csharp
public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`options` [ConnectPipeOptions](../../masstransit-abstractions/masstransit/connectpipeoptions)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
