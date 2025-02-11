---

title: MessageEntityNameFormatter<TMessage>

---

# MessageEntityNameFormatter\<TMessage\>

Namespace: MassTransit

```csharp
public class MessageEntityNameFormatter<TMessage> : IMessageEntityNameFormatter<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageEntityNameFormatter\<TMessage\>](../masstransit/messageentitynameformatter-1)<br/>
Implements [IMessageEntityNameFormatter\<TMessage\>](../masstransit/imessageentitynameformatter-1)

## Constructors

### **MessageEntityNameFormatter(IEntityNameFormatter)**

```csharp
public MessageEntityNameFormatter(IEntityNameFormatter entityNameFormatter)
```

#### Parameters

`entityNameFormatter` [IEntityNameFormatter](../masstransit/ientitynameformatter)<br/>

## Methods

### **FormatEntityName()**

Not sure it ever makes sense to pass the actual message, but many, someday.

```csharp
public string FormatEntityName()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
