---

title: IHeaderInitializer<TMessage>

---

# IHeaderInitializer\<TMessage\>

Namespace: MassTransit.Initializers

Initialize a message header

```csharp
public interface IHeaderInitializer<TMessage>
```

#### Type Parameters

`TMessage`<br/>

## Methods

### **Apply(InitializeContext\<TMessage\>, SendContext)**

```csharp
Task Apply(InitializeContext<TMessage> context, SendContext sendContext)
```

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`sendContext` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
