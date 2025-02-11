---

title: TransformPropertyConverter<TProperty>

---

# TransformPropertyConverter\<TProperty\>

Namespace: MassTransit.Transformation

```csharp
public class TransformPropertyConverter<TProperty> : IPropertyConverter<TProperty, TProperty>
```

#### Type Parameters

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransformPropertyConverter\<TProperty\>](../masstransit-transformation/transformpropertyconverter-1)<br/>
Implements [IPropertyConverter\<TProperty, TProperty\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **TransformPropertyConverter(IMessageInitializer\<TProperty\>)**

```csharp
public TransformPropertyConverter(IMessageInitializer<TProperty> initializer)
```

#### Parameters

`initializer` [IMessageInitializer\<TProperty\>](../../masstransit-abstractions/masstransit-initializers/imessageinitializer-1)<br/>

## Methods

### **Convert\<TMessage\>(InitializeContext\<TMessage\>, TProperty)**

```csharp
public Task<TProperty> Convert<TMessage>(InitializeContext<TMessage> context, TProperty input)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` TProperty<br/>

#### Returns

[Task\<TProperty\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
