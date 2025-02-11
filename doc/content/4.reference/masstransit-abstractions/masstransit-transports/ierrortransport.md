---

title: IErrorTransport

---

# IErrorTransport

Namespace: MassTransit.Transports

If present, can be used to move the [ReceiveContext](../masstransit/receivecontext) to the error queue

```csharp
public interface IErrorTransport
```

## Methods

### **Send(ExceptionReceiveContext)**

```csharp
Task Send(ExceptionReceiveContext context)
```

#### Parameters

`context` [ExceptionReceiveContext](../masstransit/exceptionreceivecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
