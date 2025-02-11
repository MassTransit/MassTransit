---

title: CopyPropertyInitializer<TMessage, TInput, TProperty>

---

# CopyPropertyInitializer\<TMessage, TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyInitializers

Set a message property by copying the input property (of the same type), regardless of whether
 the input property value is null, etc.

```csharp
public class CopyPropertyInitializer<TMessage, TInput, TProperty> : IPropertyInitializer<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CopyPropertyInitializer\<TMessage, TInput, TProperty\>](../masstransit-initializers-propertyinitializers/copypropertyinitializer-3)<br/>
Implements [IPropertyInitializer\<TMessage, TInput\>](../masstransit-initializers/ipropertyinitializer-2)

## Constructors

### **CopyPropertyInitializer(PropertyInfo, PropertyInfo)**

```csharp
public CopyPropertyInitializer(PropertyInfo messagePropertyInfo, PropertyInfo inputPropertyInfo)
```

#### Parameters

`messagePropertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`inputPropertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Apply(InitializeContext\<TMessage, TInput\>)**

```csharp
public Task Apply(InitializeContext<TMessage, TInput> context)
```

#### Parameters

`context` [InitializeContext\<TMessage, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
