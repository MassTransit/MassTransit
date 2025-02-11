---

title: DictionaryCopyPropertyInitializer<TMessage, TInput, TProperty>

---

# DictionaryCopyPropertyInitializer\<TMessage, TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyInitializers

Gets the dictionary entry for the property (if present), and sets the message property to the value

```csharp
public class DictionaryCopyPropertyInitializer<TMessage, TInput, TProperty> : IPropertyInitializer<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionaryCopyPropertyInitializer\<TMessage, TInput, TProperty\>](../masstransit-initializers-propertyinitializers/dictionarycopypropertyinitializer-3)<br/>
Implements [IPropertyInitializer\<TMessage, TInput\>](../masstransit-initializers/ipropertyinitializer-2)

## Constructors

### **DictionaryCopyPropertyInitializer(PropertyInfo, String)**

```csharp
public DictionaryCopyPropertyInitializer(PropertyInfo propertyInfo, string key)
```

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Apply(InitializeContext\<TMessage, TInput\>)**

```csharp
public Task Apply(InitializeContext<TMessage, TInput> context)
```

#### Parameters

`context` [InitializeContext\<TMessage, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
