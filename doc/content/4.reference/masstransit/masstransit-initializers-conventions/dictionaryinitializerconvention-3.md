---

title: DictionaryInitializerConvention<TMessage, TInput, TValue>

---

# DictionaryInitializerConvention\<TMessage, TInput, TValue\>

Namespace: MassTransit.Initializers.Conventions

```csharp
public class DictionaryInitializerConvention<TMessage, TInput, TValue> : IInitializerConvention<TMessage, TInput>, IMessageInputInitializerConvention<TMessage>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionaryInitializerConvention\<TMessage, TInput, TValue\>](../masstransit-initializers-conventions/dictionaryinitializerconvention-3)<br/>
Implements [IInitializerConvention\<TMessage, TInput\>](../masstransit-initializers-conventions/iinitializerconvention-2), [IMessageInputInitializerConvention\<TMessage\>](../masstransit-initializers-conventions/imessageinputinitializerconvention-1)

## Constructors

### **DictionaryInitializerConvention()**

```csharp
public DictionaryInitializerConvention()
```

## Methods

### **TryGetPropertyInitializer\<TProperty\>(PropertyInfo, IPropertyInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetPropertyInitializer<TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IPropertyInitializer\<TMessage, TInput\>](../masstransit-initializers/ipropertyinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeaderInitializer\<TProperty\>(PropertyInfo, IHeaderInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetHeaderInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeadersInitializer\<TProperty\>(PropertyInfo, IHeaderInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetHeadersInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
