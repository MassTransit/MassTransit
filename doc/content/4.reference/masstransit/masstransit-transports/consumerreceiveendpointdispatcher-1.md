---

title: ConsumerReceiveEndpointDispatcher<T>

---

# ConsumerReceiveEndpointDispatcher\<T\>

Namespace: MassTransit.Transports

```csharp
public class ConsumerReceiveEndpointDispatcher<T> : ITypeReceiveEndpointDispatcherFactory
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerReceiveEndpointDispatcher\<T\>](../masstransit-transports/consumerreceiveendpointdispatcher-1)<br/>
Implements [ITypeReceiveEndpointDispatcherFactory](../masstransit-transports/itypereceiveendpointdispatcherfactory)

## Constructors

### **ConsumerReceiveEndpointDispatcher()**

```csharp
public ConsumerReceiveEndpointDispatcher()
```

## Methods

### **Create(IReceiveEndpointDispatcherFactory, IEndpointNameFormatter)**

```csharp
public IReceiveEndpointDispatcher Create(IReceiveEndpointDispatcherFactory factory, IEndpointNameFormatter formatter)
```

#### Parameters

`factory` [IReceiveEndpointDispatcherFactory](../masstransit-transports/ireceiveendpointdispatcherfactory)<br/>

`formatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

#### Returns

[IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher)<br/>
