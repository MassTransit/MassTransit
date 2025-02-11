---

title: ReplaceMessageFactory<TMessage>

---

# ReplaceMessageFactory\<TMessage\>

Namespace: MassTransit.Transformation

```csharp
public class ReplaceMessageFactory<TMessage> : IMessageFactory<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReplaceMessageFactory\<TMessage\>](../masstransit-transformation/replacemessagefactory-1)<br/>
Implements [IMessageFactory\<TMessage\>](../masstransit-initializers/imessagefactory-1)

## Constructors

### **ReplaceMessageFactory()**

```csharp
public ReplaceMessageFactory()
```

## Methods

### **Create(InitializeContext)**

```csharp
public InitializeContext<TMessage> Create(InitializeContext context)
```

#### Parameters

`context` [InitializeContext](../../masstransit-abstractions/masstransit-initializers/initializecontext)<br/>

#### Returns

[InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>
