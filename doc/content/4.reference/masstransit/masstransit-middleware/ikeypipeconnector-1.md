---

title: IKeyPipeConnector<TKey>

---

# IKeyPipeConnector\<TKey\>

Namespace: MassTransit.Middleware

Supports connecting a pipe using a key, which is a method of dispatching to different pipes
 based on context.

```csharp
public interface IKeyPipeConnector<TKey>
```

#### Type Parameters

`TKey`<br/>

## Methods

### **ConnectPipe\<T\>(TKey, IPipe\<T\>)**

Connect a pipe to the filter using the specified key

```csharp
ConnectHandle ConnectPipe<T>(TKey key, IPipe<T> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`key` TKey<br/>

`pipe` [IPipe\<T\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
