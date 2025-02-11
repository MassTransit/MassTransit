---

title: IMessageFactory<TMessage>

---

# IMessageFactory\<TMessage\>

Namespace: MassTransit.Initializers

Creates the message type

```csharp
public interface IMessageFactory<TMessage>
```

#### Type Parameters

`TMessage`<br/>
The message type

## Methods

### **Create(InitializeContext)**

```csharp
InitializeContext<TMessage> Create(InitializeContext context)
```

#### Parameters

`context` [InitializeContext](../../masstransit-abstractions/masstransit-initializers/initializecontext)<br/>

#### Returns

[InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>
