---

title: SetHeaderInitializer<TMessage, TInput, THeader>

---

# SetHeaderInitializer\<TMessage, TInput, THeader\>

Namespace: MassTransit.Initializers.HeaderInitializers

Set a header to a constant value from the input

```csharp
public class SetHeaderInitializer<TMessage, TInput, THeader> : IHeaderInitializer<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`THeader`<br/>
The header type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetHeaderInitializer\<TMessage, TInput, THeader\>](../masstransit-initializers-headerinitializers/setheaderinitializer-3)<br/>
Implements [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)

## Constructors

### **SetHeaderInitializer(String, IPropertyProvider\<TInput, THeader\>)**

```csharp
public SetHeaderInitializer(string headerName, IPropertyProvider<TInput, THeader> provider)
```

#### Parameters

`headerName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`provider` [IPropertyProvider\<TInput, THeader\>](../masstransit-initializers/ipropertyprovider-2)<br/>

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
