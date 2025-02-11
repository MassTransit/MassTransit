---

title: ITypeReceiveEndpointDispatcherFactory

---

# ITypeReceiveEndpointDispatcherFactory

Namespace: MassTransit.Transports

```csharp
public interface ITypeReceiveEndpointDispatcherFactory
```

## Methods

### **Create(IReceiveEndpointDispatcherFactory, IEndpointNameFormatter)**

```csharp
IReceiveEndpointDispatcher Create(IReceiveEndpointDispatcherFactory factory, IEndpointNameFormatter formatter)
```

#### Parameters

`factory` [IReceiveEndpointDispatcherFactory](../masstransit-transports/ireceiveendpointdispatcherfactory)<br/>

`formatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

#### Returns

[IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher)<br/>
