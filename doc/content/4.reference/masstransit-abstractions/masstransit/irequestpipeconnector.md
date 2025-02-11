---

title: IRequestPipeConnector

---

# IRequestPipeConnector

Namespace: MassTransit

Connect a request pipe to the pipeline

```csharp
public interface IRequestPipeConnector
```

## Methods

### **ConnectRequestPipe\<T\>(Guid, IPipe\<ConsumeContext\<T\>\>)**

Connect the consume pipe to the pipeline for messages with the specified RequestId header

```csharp
ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<T\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
