---

title: IMessageRoutingKeyFormatter<TMessage>

---

# IMessageRoutingKeyFormatter\<TMessage\>

Namespace: MassTransit.Transports

```csharp
public interface IMessageRoutingKeyFormatter<TMessage>
```

#### Type Parameters

`TMessage`<br/>

## Methods

### **FormatRoutingKey(SendContext\<TMessage\>)**

```csharp
string FormatRoutingKey(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
