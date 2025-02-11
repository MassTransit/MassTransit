---

title: MessageInitializerBuilder<TMessage, TInput>

---

# MessageInitializerBuilder\<TMessage, TInput\>

Namespace: MassTransit.Initializers.Factories

```csharp
public class MessageInitializerBuilder<TMessage, TInput> : IMessageInitializerBuilder<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageInitializerBuilder\<TMessage, TInput\>](../masstransit-initializers-factories/messageinitializerbuilder-2)<br/>
Implements [IMessageInitializerBuilder\<TMessage, TInput\>](../masstransit-initializers-factories/imessageinitializerbuilder-2)

## Constructors

### **MessageInitializerBuilder(IMessageFactory\<TMessage\>)**

```csharp
public MessageInitializerBuilder(IMessageFactory<TMessage> messageFactory)
```

#### Parameters

`messageFactory` [IMessageFactory\<TMessage\>](../masstransit-initializers/imessagefactory-1)<br/>

## Methods

### **Add(String, IPropertyInitializer\<TMessage\>)**

```csharp
public void Add(string propertyName, IPropertyInitializer<TMessage> initializer)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`initializer` [IPropertyInitializer\<TMessage\>](../masstransit-initializers/ipropertyinitializer-1)<br/>

### **Add(String, IPropertyInitializer\<TMessage, TInput\>)**

```csharp
public void Add(string propertyName, IPropertyInitializer<TMessage, TInput> initializer)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`initializer` [IPropertyInitializer\<TMessage, TInput\>](../masstransit-initializers/ipropertyinitializer-2)<br/>

### **Add(IHeaderInitializer\<TMessage\>)**

```csharp
public void Add(IHeaderInitializer<TMessage> initializer)
```

#### Parameters

`initializer` [IHeaderInitializer\<TMessage\>](../masstransit-initializers/iheaderinitializer-1)<br/>

### **Add(IHeaderInitializer\<TMessage, TInput\>)**

```csharp
public void Add(IHeaderInitializer<TMessage, TInput> initializer)
```

#### Parameters

`initializer` [IHeaderInitializer\<TMessage, TInput\>](../masstransit-initializers/iheaderinitializer-2)<br/>

### **IsInputPropertyUsed(String)**

```csharp
public bool IsInputPropertyUsed(string propertyName)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SetInputPropertyUsed(String)**

```csharp
public void SetInputPropertyUsed(string propertyName)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Build()**

```csharp
public IMessageInitializer<TMessage> Build()
```

#### Returns

[IMessageInitializer\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/imessageinitializer-1)<br/>
