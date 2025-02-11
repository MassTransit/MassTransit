---

title: MessageInitializerFactory<TMessage, TInput>

---

# MessageInitializerFactory\<TMessage, TInput\>

Namespace: MassTransit.Initializers.Factories

```csharp
public class MessageInitializerFactory<TMessage, TInput> : IMessageInitializerFactory<TMessage>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageInitializerFactory\<TMessage, TInput\>](../masstransit-initializers-factories/messageinitializerfactory-2)<br/>
Implements [IMessageInitializerFactory\<TMessage\>](../masstransit-initializers/imessageinitializerfactory-1)

## Constructors

### **MessageInitializerFactory(IInitializerConvention[])**

```csharp
public MessageInitializerFactory(IInitializerConvention[] conventions)
```

#### Parameters

`conventions` [IInitializerConvention[]](../masstransit-initializers-conventions/iinitializerconvention)<br/>

### **MessageInitializerFactory(IMessageFactory\<TMessage\>, IInitializerConvention[])**

```csharp
public MessageInitializerFactory(IMessageFactory<TMessage> messageFactory, IInitializerConvention[] conventions)
```

#### Parameters

`messageFactory` [IMessageFactory\<TMessage\>](../masstransit-initializers/imessagefactory-1)<br/>

`conventions` [IInitializerConvention[]](../masstransit-initializers-conventions/iinitializerconvention)<br/>

## Methods

### **CreateMessageInitializer()**

```csharp
public IMessageInitializer<TMessage> CreateMessageInitializer()
```

#### Returns

[IMessageInitializer\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/imessageinitializer-1)<br/>
