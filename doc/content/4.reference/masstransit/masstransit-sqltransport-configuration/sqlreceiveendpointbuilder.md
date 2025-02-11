---

title: SqlReceiveEndpointBuilder

---

# SqlReceiveEndpointBuilder

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlReceiveEndpointBuilder : ReceiveEndpointBuilder, IReceiveEndpointBuilder, IConsumePipeConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointBuilder](../masstransit-configuration/receiveendpointbuilder) → [SqlReceiveEndpointBuilder](../masstransit-sqltransport-configuration/sqlreceiveendpointbuilder)<br/>
Implements [IReceiveEndpointBuilder](../../masstransit-abstractions/masstransit-configuration/ireceiveendpointbuilder), [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)

## Constructors

### **SqlReceiveEndpointBuilder(ISqlHostConfiguration, ISqlReceiveEndpointConfiguration)**

```csharp
public SqlReceiveEndpointBuilder(ISqlHostConfiguration hostConfiguration, ISqlReceiveEndpointConfiguration configuration)
```

#### Parameters

`hostConfiguration` [ISqlHostConfiguration](../masstransit-sqltransport-configuration/isqlhostconfiguration)<br/>

`configuration` [ISqlReceiveEndpointConfiguration](../masstransit-sqltransport-configuration/isqlreceiveendpointconfiguration)<br/>

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
public SqlReceiveEndpointContext CreateReceiveEndpointContext()
```

#### Returns

[SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>
