---

title: DictionaryCopyHeaderInitializer<TMessage, TInput, THeader>

---

# DictionaryCopyHeaderInitializer\<TMessage, TInput, THeader\>

Namespace: MassTransit.Initializers.HeaderInitializers

```csharp
public class DictionaryCopyHeaderInitializer<TMessage, TInput, THeader> : IHeaderInitializer<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`THeader`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionaryCopyHeaderInitializer\<TMessage, TInput, THeader\>](../masstransit-initializers-headerinitializers/dictionarycopyheaderinitializer-3)<br/>
Implements [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)

## Constructors

### **DictionaryCopyHeaderInitializer(PropertyInfo, String)**

```csharp
public DictionaryCopyHeaderInitializer(PropertyInfo propertyInfo, string key)
```

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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
