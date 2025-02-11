---

title: InMemoryReceiveEndpointBuilder

---

# InMemoryReceiveEndpointBuilder

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public class InMemoryReceiveEndpointBuilder : ReceiveEndpointBuilder, IReceiveEndpointBuilder, IConsumePipeConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointBuilder](../masstransit-configuration/receiveendpointbuilder) → [InMemoryReceiveEndpointBuilder](../masstransit-inmemorytransport-configuration/inmemoryreceiveendpointbuilder)<br/>
Implements [IReceiveEndpointBuilder](../../masstransit-abstractions/masstransit-configuration/ireceiveendpointbuilder), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)

## Constructors

### **InMemoryReceiveEndpointBuilder(IInMemoryHostConfiguration, IInMemoryReceiveEndpointConfiguration)**

```csharp
public InMemoryReceiveEndpointBuilder(IInMemoryHostConfiguration hostConfiguration, IInMemoryReceiveEndpointConfiguration configuration)
```

#### Parameters

`hostConfiguration` [IInMemoryHostConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryhostconfiguration)<br/>

`configuration` [IInMemoryReceiveEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryreceiveendpointconfiguration)<br/>

## Methods

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

### **CreateReceiveEndpointContext()**

```csharp
public InMemoryReceiveEndpointContext CreateReceiveEndpointContext()
```

#### Returns

[InMemoryReceiveEndpointContext](../masstransit-inmemorytransport/inmemoryreceiveendpointcontext)<br/>
