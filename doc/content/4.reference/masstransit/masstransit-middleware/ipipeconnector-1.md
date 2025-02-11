---

title: IPipeConnector<TContext>

---

# IPipeConnector\<TContext\>

Namespace: MassTransit.Middleware

Connect a pipe of the same type as the target pipe

```csharp
public interface IPipeConnector<TContext>
```

#### Type Parameters

`TContext`<br/>

## Methods

### **ConnectPipe(IPipe\<TContext\>)**

```csharp
ConnectHandle ConnectPipe(IPipe<TContext> pipe)
```

#### Parameters

`pipe` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
