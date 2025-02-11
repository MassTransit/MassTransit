---

title: IInitializerVariable<T>

---

# IInitializerVariable\<T\>

Namespace: MassTransit.Initializers

```csharp
public interface IInitializerVariable<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **GetValue\<TMessage\>(InitializeContext\<TMessage\>)**

```csharp
Task<T> GetValue<TMessage>(InitializeContext<TMessage> context)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [InitializeContext\<TMessage\>](../masstransit-initializers/initializecontext-1)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
