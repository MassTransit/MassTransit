---

title: DynamicMessageFactory<TMessage, TImplementation>

---

# DynamicMessageFactory\<TMessage, TImplementation\>

Namespace: MassTransit.Initializers.Factories

```csharp
public class DynamicMessageFactory<TMessage, TImplementation> : IMessageFactory<TMessage>
```

#### Type Parameters

`TMessage`<br/>

`TImplementation`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DynamicMessageFactory\<TMessage, TImplementation\>](../masstransit-initializers-factories/dynamicmessagefactory-2)<br/>
Implements [IMessageFactory\<TMessage\>](../masstransit-initializers/imessagefactory-1)

## Constructors

### **DynamicMessageFactory()**

```csharp
public DynamicMessageFactory()
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
