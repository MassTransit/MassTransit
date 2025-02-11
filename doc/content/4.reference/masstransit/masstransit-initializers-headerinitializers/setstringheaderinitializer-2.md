---

title: SetStringHeaderInitializer<TMessage, TInput>

---

# SetStringHeaderInitializer\<TMessage, TInput\>

Namespace: MassTransit.Initializers.HeaderInitializers

Set a header to a constant value from the input

```csharp
public class SetStringHeaderInitializer<TMessage, TInput> : IHeaderInitializer<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetStringHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers-headerinitializers/setstringheaderinitializer-2)<br/>
Implements [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)

## Constructors

### **SetStringHeaderInitializer(String, PropertyInfo)**

```csharp
public SetStringHeaderInitializer(string headerName, PropertyInfo propertyInfo)
```

#### Parameters

`headerName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Apply(InitializeContext\<TMessage, TInput\>, SendContext)**

```csharp
public Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext)
```

#### Parameters

`context` [InitializeContext\<TMessage, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

`sendContext` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
