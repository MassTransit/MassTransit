---

title: IMessagePartitionKeyFormatter<TMessage>

---

# IMessagePartitionKeyFormatter\<TMessage\>

Namespace: MassTransit.Transports

```csharp
public interface IMessagePartitionKeyFormatter<TMessage>
```

#### Type Parameters

`TMessage`<br/>

## Methods

### **FormatPartitionKey(SendContext\<TMessage\>)**

```csharp
string FormatPartitionKey(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
