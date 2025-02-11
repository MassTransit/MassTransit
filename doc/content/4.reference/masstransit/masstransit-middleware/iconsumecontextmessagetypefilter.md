---

title: IConsumeContextMessageTypeFilter

---

# IConsumeContextMessageTypeFilter

Namespace: MassTransit.Middleware

```csharp
public interface IConsumeContextMessageTypeFilter : IFilter<ConsumeContext>, IProbeSite, IConsumeMessageObserverConnector, IConsumeObserverConnector
```

Implements [IFilter\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector)

## Methods

### **ConnectMessagePipe\<T\>(IPipe\<ConsumeContext\<T\>\>)**

```csharp
ConnectHandle ConnectMessagePipe<T>(IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectMessagePipe\<T\>(Guid, IPipe\<ConsumeContext\<T\>\>)**

```csharp
ConnectHandle ConnectMessagePipe<T>(Guid key, IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`key` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`pipe` [IPipe\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
