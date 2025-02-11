---

title: CopyHeaderInitializer<TMessage, TInput, THeader>

---

# CopyHeaderInitializer\<TMessage, TInput, THeader\>

Namespace: MassTransit.Initializers.HeaderInitializers

Set a header to a constant value from the input

```csharp
public class CopyHeaderInitializer<TMessage, TInput, THeader> : IHeaderInitializer<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`THeader`<br/>
The header type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CopyHeaderInitializer\<TMessage, TInput, THeader\>](../masstransit-initializers-headerinitializers/copyheaderinitializer-3)<br/>
Implements [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)

## Constructors

### **CopyHeaderInitializer(PropertyInfo, PropertyInfo)**

```csharp
public CopyHeaderInitializer(PropertyInfo headerPropertyInfo, PropertyInfo inputPropertyInfo)
```

#### Parameters

`headerPropertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`inputPropertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

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
