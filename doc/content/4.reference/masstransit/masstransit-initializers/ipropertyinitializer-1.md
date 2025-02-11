---

title: IPropertyInitializer<TMessage>

---

# IPropertyInitializer\<TMessage\>

Namespace: MassTransit.Initializers

A message initializer that doesn't use the input

```csharp
public interface IPropertyInitializer<TMessage>
```

#### Type Parameters

`TMessage`<br/>
The message type

## Methods

### **Apply(InitializeContext\<TMessage\>)**

Apply the initializer to the message

```csharp
Task Apply(InitializeContext<TMessage> context)
```

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
