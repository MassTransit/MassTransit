---

title: IMessageInitializerBuilder<TMessage, TInput>

---

# IMessageInitializerBuilder\<TMessage, TInput\>

Namespace: MassTransit.Initializers.Factories

```csharp
public interface IMessageInitializerBuilder<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

## Methods

### **Add(String, IPropertyInitializer\<TMessage\>)**

```csharp
void Add(string propertyName, IPropertyInitializer<TMessage> initializer)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`initializer` [IPropertyInitializer\<TMessage\>](../masstransit-initializers/ipropertyinitializer-1)<br/>

### **Add(String, IPropertyInitializer\<TMessage, TInput\>)**

```csharp
void Add(string propertyName, IPropertyInitializer<TMessage, TInput> initializer)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`initializer` [IPropertyInitializer\<TMessage, TInput\>](../masstransit-initializers/ipropertyinitializer-2)<br/>

### **Add(IHeaderInitializer\<TMessage\>)**

```csharp
void Add(IHeaderInitializer<TMessage> initializer)
```

#### Parameters

`initializer` [IHeaderInitializer\<TMessage\>](../masstransit-initializers/iheaderinitializer-1)<br/>

### **Add(IHeaderInitializer\<TMessage, TInput\>)**

```csharp
void Add(IHeaderInitializer<TMessage, TInput> initializer)
```

#### Parameters

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

### **IsInputPropertyUsed(String)**

```csharp
bool IsInputPropertyUsed(string propertyName)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SetInputPropertyUsed(String)**

```csharp
void SetInputPropertyUsed(string propertyName)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
