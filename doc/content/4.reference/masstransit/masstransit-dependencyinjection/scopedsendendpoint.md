---

title: ScopedSendEndpoint

---

# ScopedSendEndpoint

Namespace: MassTransit.DependencyInjection

```csharp
public class ScopedSendEndpoint : SendEndpointProxy, ITransportSendEndpoint, ISendEndpoint, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendEndpointProxy](../masstransit-transports/sendendpointproxy) → [ScopedSendEndpoint](../masstransit-dependencyinjection/scopedsendendpoint)<br/>
Implements [ITransportSendEndpoint](../masstransit-transports/itransportsendendpoint), [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Properties

### **Endpoint**

```csharp
public ISendEndpoint Endpoint { get; }
```

#### Property Value

[ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

## Constructors

### **ScopedSendEndpoint(ISendEndpoint, IServiceProvider)**

```csharp
public ScopedSendEndpoint(ISendEndpoint endpoint, IServiceProvider scope)
```

#### Parameters

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

`scope` IServiceProvider<br/>

## Methods

### **GetPipeProxy\<T\>(IPipe\<SendContext\<T\>\>)**

```csharp
protected IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
