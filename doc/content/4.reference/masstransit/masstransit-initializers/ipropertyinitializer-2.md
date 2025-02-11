---

title: IPropertyInitializer<TMessage, TInput>

---

# IPropertyInitializer\<TMessage, TInput\>

Namespace: MassTransit.Initializers

A message initializer that uses the input

```csharp
public interface IPropertyInitializer<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>
The message type

`TInput`<br/>
The input type

## Methods

### **Apply(InitializeContext\<TMessage, TInput\>)**

```csharp
Task Apply(InitializeContext<TMessage, TInput> context)
```

#### Parameters

`context` [InitializeContext\<TMessage, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
