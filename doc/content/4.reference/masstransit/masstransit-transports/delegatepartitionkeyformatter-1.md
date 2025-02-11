---

title: DelegatePartitionKeyFormatter<TMessage>

---

# DelegatePartitionKeyFormatter\<TMessage\>

Namespace: MassTransit.Transports

```csharp
public class DelegatePartitionKeyFormatter<TMessage> : IMessagePartitionKeyFormatter<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegatePartitionKeyFormatter\<TMessage\>](../masstransit-transports/delegatepartitionkeyformatter-1)<br/>
Implements [IMessagePartitionKeyFormatter\<TMessage\>](../masstransit-transports/imessagepartitionkeyformatter-1)

## Constructors

### **DelegatePartitionKeyFormatter(Func\<SendContext\<TMessage\>, String\>)**

```csharp
public DelegatePartitionKeyFormatter(Func<SendContext<TMessage>, string> formatter)
```

#### Parameters

`formatter` [Func\<SendContext\<TMessage\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **FormatPartitionKey(SendContext\<TMessage\>)**

```csharp
public string FormatPartitionKey(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
