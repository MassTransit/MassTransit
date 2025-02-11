---

title: PublishPipe

---

# PublishPipe

Namespace: MassTransit.Middleware

```csharp
public class PublishPipe : IPublishPipe, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishPipe](../masstransit-middleware/publishpipe)<br/>
Implements [IPublishPipe](../masstransit-transports/ipublishpipe), [IProbeSite](../masstransit/iprobesite)

## Constructors

### **PublishPipe(IPublishPipeSpecification)**

```csharp
public PublishPipe(IPublishPipeSpecification specification)
```

#### Parameters

`specification` [IPublishPipeSpecification](../masstransit-configuration/ipublishpipespecification)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../masstransit/probecontext)<br/>

### **Send\<T\>(PublishContext\<T\>)**

```csharp
public Task Send<T>(PublishContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../masstransit/publishcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
