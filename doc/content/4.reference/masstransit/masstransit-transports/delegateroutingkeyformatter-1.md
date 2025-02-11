---

title: DelegateRoutingKeyFormatter<TMessage>

---

# DelegateRoutingKeyFormatter\<TMessage\>

Namespace: MassTransit.Transports

```csharp
public class DelegateRoutingKeyFormatter<TMessage> : IMessageRoutingKeyFormatter<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegateRoutingKeyFormatter\<TMessage\>](../masstransit-transports/delegateroutingkeyformatter-1)<br/>
Implements [IMessageRoutingKeyFormatter\<TMessage\>](../masstransit-transports/imessageroutingkeyformatter-1)

## Constructors

### **DelegateRoutingKeyFormatter(Func\<SendContext\<TMessage\>, String\>)**

```csharp
public DelegateRoutingKeyFormatter(Func<SendContext<TMessage>, string> formatter)
```

#### Parameters

`formatter` [Func\<SendContext\<TMessage\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **FormatRoutingKey(SendContext\<TMessage\>)**

```csharp
public string FormatRoutingKey(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
