---

title: SagaReceiveEndpointDispatcher<T>

---

# SagaReceiveEndpointDispatcher\<T\>

Namespace: MassTransit.Transports

```csharp
public class SagaReceiveEndpointDispatcher<T> : ITypeReceiveEndpointDispatcherFactory
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaReceiveEndpointDispatcher\<T\>](../masstransit-transports/sagareceiveendpointdispatcher-1)<br/>
Implements [ITypeReceiveEndpointDispatcherFactory](../masstransit-transports/itypereceiveendpointdispatcherfactory)

## Constructors

### **SagaReceiveEndpointDispatcher()**

```csharp
public SagaReceiveEndpointDispatcher()
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
