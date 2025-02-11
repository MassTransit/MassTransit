---

title: IKeyPipeConnector<TMessage, TKey>

---

# IKeyPipeConnector\<TMessage, TKey\>

Namespace: MassTransit.Middleware

Supports connecting a pipe using a key, which is a method of dispatching to different pipes
 based on context.

```csharp
public interface IKeyPipeConnector<TMessage, TKey>
```

#### Type Parameters

`TMessage`<br/>

`TKey`<br/>

## Methods

### **ConnectPipe(TKey, IPipe\<ConsumeContext\<TMessage\>\>)**

Connect a pipe to the filter using the specified key

```csharp
ConnectHandle ConnectPipe(TKey key, IPipe<ConsumeContext<TMessage>> pipe)
```

#### Parameters

`key` TKey<br/>

`pipe` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
