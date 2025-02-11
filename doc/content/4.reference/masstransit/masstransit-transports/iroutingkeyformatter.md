---

title: IRoutingKeyFormatter

---

# IRoutingKeyFormatter

Namespace: MassTransit.Transports

```csharp
public interface IRoutingKeyFormatter
```

## Methods

### **FormatRoutingKey\<T\>(SendContext\<T\>)**

Format the routing key to be used by the transport, if supported

```csharp
string FormatRoutingKey<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>
The message send context

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The routing key to specify in the transport
