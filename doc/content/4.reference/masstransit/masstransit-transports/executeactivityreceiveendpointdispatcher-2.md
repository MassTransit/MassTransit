---

title: ExecuteActivityReceiveEndpointDispatcher<TActivity, TArguments>

---

# ExecuteActivityReceiveEndpointDispatcher\<TActivity, TArguments\>

Namespace: MassTransit.Transports

```csharp
public class ExecuteActivityReceiveEndpointDispatcher<TActivity, TArguments> : ITypeReceiveEndpointDispatcherFactory
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityReceiveEndpointDispatcher\<TActivity, TArguments\>](../masstransit-transports/executeactivityreceiveendpointdispatcher-2)<br/>
Implements [ITypeReceiveEndpointDispatcherFactory](../masstransit-transports/itypereceiveendpointdispatcherfactory)

## Constructors

### **ExecuteActivityReceiveEndpointDispatcher()**

```csharp
public ExecuteActivityReceiveEndpointDispatcher()
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
