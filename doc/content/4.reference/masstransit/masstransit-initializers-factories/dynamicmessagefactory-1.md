---

title: DynamicMessageFactory<TMessage>

---

# DynamicMessageFactory\<TMessage\>

Namespace: MassTransit.Initializers.Factories

```csharp
public class DynamicMessageFactory<TMessage> : IMessageFactory<TMessage>, IMessageFactory
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DynamicMessageFactory\<TMessage\>](../masstransit-initializers-factories/dynamicmessagefactory-1)<br/>
Implements [IMessageFactory\<TMessage\>](../masstransit-initializers/imessagefactory-1), [IMessageFactory](../masstransit-initializers/imessagefactory)

## Constructors

### **DynamicMessageFactory()**

```csharp
public DynamicMessageFactory()
```

## Methods

### **Create()**

```csharp
public object Create()
```

#### Returns

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **Create(InitializeContext)**

```csharp
public InitializeContext<TMessage> Create(InitializeContext context)
```

#### Parameters

`context` [InitializeContext](../../masstransit-abstractions/masstransit-initializers/initializecontext)<br/>

#### Returns

[InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>
