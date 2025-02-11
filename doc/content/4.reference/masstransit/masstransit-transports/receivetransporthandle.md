---

title: ReceiveTransportHandle

---

# ReceiveTransportHandle

Namespace: MassTransit.Transports

A handle to an active transport

```csharp
public interface ReceiveTransportHandle
```

## Methods

### **Stop(CancellationToken)**

Stop the transport, releasing any resources associated with the endpoint

```csharp
Task Stop(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
