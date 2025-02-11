---

title: DefaultInitializerConvention<TMessage, TInput>

---

# DefaultInitializerConvention\<TMessage, TInput\>

Namespace: MassTransit.Initializers.Conventions

```csharp
public class DefaultInitializerConvention<TMessage, TInput> : IInitializerConvention<TMessage, TInput>, IMessageInputInitializerConvention<TMessage>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultInitializerConvention\<TMessage, TInput\>](../masstransit-initializers-conventions/defaultinitializerconvention-2)<br/>
Implements [IInitializerConvention\<TMessage, TInput\>](../masstransit-initializers-conventions/iinitializerconvention-2), [IMessageInputInitializerConvention\<TMessage\>](../masstransit-initializers-conventions/imessageinputinitializerconvention-1)

## Constructors

### **DefaultInitializerConvention()**

```csharp
public DefaultInitializerConvention()
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
