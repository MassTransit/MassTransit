---

title: IPipeConnector

---

# IPipeConnector

Namespace: MassTransit.Middleware

The intent is to connect a pipe of a specific type to a pipe of a different type,
 for which there is a provider that knows how to convert the input type to the output type.

```csharp
public interface IPipeConnector
```

## Methods

### **ConnectPipe\<T\>(IPipe\<T\>)**

Connect a pipe of the specified type to the DispatchFilter

```csharp
ConnectHandle ConnectPipe<T>(IPipe<T> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<T\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
