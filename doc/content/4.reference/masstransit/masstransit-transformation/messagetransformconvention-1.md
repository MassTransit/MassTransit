---

title: MessageTransformConvention<TMessage>

---

# MessageTransformConvention\<TMessage\>

Namespace: MassTransit.Transformation

```csharp
public class MessageTransformConvention<TMessage> : IInitializerConvention<TMessage, TMessage>, IMessageInputInitializerConvention<TMessage>, IInitializerConvention<TMessage>, IMessageInitializerConvention, IInitializerConvention
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageTransformConvention\<TMessage\>](../masstransit-transformation/messagetransformconvention-1)<br/>
Implements [IInitializerConvention\<TMessage, TMessage\>](../masstransit-initializers-conventions/iinitializerconvention-2), [IMessageInputInitializerConvention\<TMessage\>](../masstransit-initializers-conventions/imessageinputinitializerconvention-1), [IInitializerConvention\<TMessage\>](../masstransit-initializers-conventions/iinitializerconvention-1), [IMessageInitializerConvention](../masstransit-initializers-conventions/imessageinitializerconvention), [IInitializerConvention](../masstransit-initializers-conventions/iinitializerconvention)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **MessageTransformConvention()**

```csharp
public MessageTransformConvention()
```

## Methods

### **TryGetPropertyInitializer\<T, TInput, TProperty\>(PropertyInfo, IPropertyInitializer\<T, TInput\>)**

```csharp
public bool TryGetPropertyInitializer<T, TInput, TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<T, TInput> initializer)
```

#### Type Parameters

`T`<br/>

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IPropertyInitializer\<T, TInput\>](../masstransit-initializers/ipropertyinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeaderInitializer\<T, TInput, TProperty\>(PropertyInfo, IHeaderInitializer\<T, TInput\>)**

```csharp
public bool TryGetHeaderInitializer<T, TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<T, TInput> initializer)
```

#### Type Parameters

`T`<br/>

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<T, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeadersInitializer\<T, TInput, TProperty\>(PropertyInfo, IHeaderInitializer\<T, TInput\>)**

```csharp
public bool TryGetHeadersInitializer<T, TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<T, TInput> initializer)
```

#### Type Parameters

`T`<br/>

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<T, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPropertyInitializer\<TProperty\>(PropertyInfo, IPropertyInitializer\<TMessage, TMessage\>)**

```csharp
public bool TryGetPropertyInitializer<TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TMessage> initializer)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IPropertyInitializer\<TMessage, TMessage\>](../masstransit-initializers/ipropertyinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeaderInitializer\<TProperty\>(PropertyInfo, IHeaderInitializer\<TMessage, TMessage\>)**

```csharp
public bool TryGetHeaderInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TMessage> initializer)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TMessage\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeadersInitializer\<TProperty\>(PropertyInfo, IHeaderInitializer\<TMessage, TMessage\>)**

```csharp
public bool TryGetHeadersInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TMessage> initializer)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TMessage\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPropertyInitializer\<TInput, TProperty\>(PropertyInfo, IPropertyInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetPropertyInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IPropertyInitializer\<TMessage, TInput\>](../masstransit-initializers/ipropertyinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeaderInitializer\<TInput, TProperty\>(PropertyInfo, IHeaderInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetHeaderInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeadersInitializer\<TInput, TProperty\>(PropertyInfo, IHeaderInitializer\<TMessage, TInput\>)**

```csharp
public bool TryGetHeadersInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Add(String, IPropertyInitializer\<TMessage, TMessage\>)**

```csharp
public void Add(string propertyName, IPropertyInitializer<TMessage, TMessage> initializer)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`initializer` [IPropertyInitializer\<TMessage, TMessage\>](../masstransit-initializers/ipropertyinitializer-2)<br/>
