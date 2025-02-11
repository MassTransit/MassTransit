---

title: ReceiveTransportObserverExtensions

---

# ReceiveTransportObserverExtensions

Namespace: MassTransit.Transports

```csharp
public static class ReceiveTransportObserverExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveTransportObserverExtensions](../masstransit-transports/receivetransportobserverextensions)

## Methods

### **NotifyReady(IReceiveTransportObserver, Uri, Boolean)**

```csharp
public static Task NotifyReady(IReceiveTransportObserver observer, Uri inputAddress, bool isStarted)
```

#### Parameters

`observer` [IReceiveTransportObserver](../../masstransit-abstractions/masstransit/ireceivetransportobserver)<br/>

`inputAddress` Uri<br/>

`isStarted` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyCompleted(IReceiveTransportObserver, Uri, DeliveryMetrics)**

```csharp
public static Task NotifyCompleted(IReceiveTransportObserver observer, Uri inputAddress, DeliveryMetrics metrics)
```

#### Parameters

`observer` [IReceiveTransportObserver](../../masstransit-abstractions/masstransit/ireceivetransportobserver)<br/>

`inputAddress` Uri<br/>

`metrics` [DeliveryMetrics](../masstransit-transports/deliverymetrics)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted(IReceiveTransportObserver, Uri, Exception)**

```csharp
public static Task NotifyFaulted(IReceiveTransportObserver observer, Uri inputAddress, Exception exception)
```

#### Parameters

`observer` [IReceiveTransportObserver](../../masstransit-abstractions/masstransit/ireceivetransportobserver)<br/>

`inputAddress` Uri<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
