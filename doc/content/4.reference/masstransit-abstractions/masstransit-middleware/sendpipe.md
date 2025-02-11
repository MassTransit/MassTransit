---

title: SendPipe

---

# SendPipe

Namespace: MassTransit.Middleware

```csharp
public class SendPipe : ISendPipe, ISendContextPipe, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendPipe](../masstransit-middleware/sendpipe)<br/>
Implements [ISendPipe](../masstransit-transports/isendpipe), [ISendContextPipe](../masstransit-transports/isendcontextpipe), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **SendPipe(ISendPipeSpecification)**

```csharp
public SendPipe(ISendPipeSpecification specification)
```

#### Parameters

`specification` [ISendPipeSpecification](../masstransit-configuration/isendpipespecification)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../masstransit/probecontext)<br/>

### **Send\<T\>(SendContext\<T\>)**

```csharp
public Task Send<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
