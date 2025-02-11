---

title: TransportStartExtensions

---

# TransportStartExtensions

Namespace: MassTransit.Transports

```csharp
public static class TransportStartExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransportStartExtensions](../masstransit-transports/transportstartextensions)

## Methods

### **OnTransportStartup\<T\>(ReceiveEndpointContext, ITransportSupervisor\<T\>, CancellationToken)**

```csharp
public static Task OnTransportStartup<T>(ReceiveEndpointContext context, ITransportSupervisor<T> supervisor, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`supervisor` [ITransportSupervisor\<T\>](../../masstransit-abstractions/masstransit-transports/itransportsupervisor-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
