---

title: IInitializerConvention<TMessage, TInput>

---

# IInitializerConvention\<TMessage, TInput\>

Namespace: MassTransit.Initializers.Conventions

```csharp
public interface IInitializerConvention<TMessage, TInput> : IMessageInputInitializerConvention<TMessage>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

Implements [IMessageInputInitializerConvention\<TMessage\>](../masstransit-initializers-conventions/imessageinputinitializerconvention-1)

## Methods

### **TryGetPropertyInitializer\<TProperty\>(PropertyInfo, IPropertyInitializer\<TMessage, TInput\>)**

```csharp
bool TryGetPropertyInitializer<TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
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
bool TryGetHeaderInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
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
bool TryGetHeadersInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
