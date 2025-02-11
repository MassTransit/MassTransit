---

title: CopyObjectPropertyInitializer<TMessage, TInput, TInputProperty>

---

# CopyObjectPropertyInitializer\<TMessage, TInput, TInputProperty\>

Namespace: MassTransit.Initializers.PropertyInitializers

```csharp
public class CopyObjectPropertyInitializer<TMessage, TInput, TInputProperty> : IPropertyInitializer<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TInputProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CopyObjectPropertyInitializer\<TMessage, TInput, TInputProperty\>](../masstransit-initializers-propertyinitializers/copyobjectpropertyinitializer-3)<br/>
Implements [IPropertyInitializer\<TMessage, TInput\>](../masstransit-initializers/ipropertyinitializer-2)

## Constructors

### **CopyObjectPropertyInitializer(PropertyInfo, PropertyInfo)**

```csharp
public CopyObjectPropertyInitializer(PropertyInfo messagePropertyInfo, PropertyInfo inputPropertyInfo)
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
