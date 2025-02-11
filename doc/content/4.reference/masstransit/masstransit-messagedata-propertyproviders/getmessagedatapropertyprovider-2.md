---

title: GetMessageDataPropertyProvider<TInput, TValue>

---

# GetMessageDataPropertyProvider\<TInput, TValue\>

Namespace: MassTransit.MessageData.PropertyProviders

```csharp
public class GetMessageDataPropertyProvider<TInput, TValue> : IPropertyProvider<TInput, MessageData<TValue>>
```

#### Type Parameters

`TInput`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GetMessageDataPropertyProvider\<TInput, TValue\>](../masstransit-messagedata-propertyproviders/getmessagedatapropertyprovider-2)<br/>
Implements [IPropertyProvider\<TInput, MessageData\<TValue\>\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **GetMessageDataPropertyProvider(IPropertyProvider\<TInput, MessageData\<TValue\>\>, IMessageDataRepository)**

```csharp
public GetMessageDataPropertyProvider(IPropertyProvider<TInput, MessageData<TValue>> inputProvider, IMessageDataRepository repository)
```

#### Parameters

`inputProvider` [IPropertyProvider\<TInput, MessageData\<TValue\>\>](../masstransit-initializers/ipropertyprovider-2)<br/>

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

## Methods

### **GetProperty\<T\>(InitializeContext\<T, TInput\>)**

```csharp
public Task<MessageData<TValue>> GetProperty<T>(InitializeContext<T, TInput> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

#### Returns

[Task\<MessageData\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
