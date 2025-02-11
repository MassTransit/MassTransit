---

title: MessageDataPropertyConverter<TValue>

---

# MessageDataPropertyConverter\<TValue\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class MessageDataPropertyConverter<TValue> : IPropertyConverter<MessageData<TValue>, MessageData<TValue>>, IPropertyConverter<MessageData<TValue>, TValue>
```

#### Type Parameters

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataPropertyConverter\<TValue\>](../masstransit-initializers-propertyconverters/messagedatapropertyconverter-1)<br/>
Implements [IPropertyConverter\<MessageData\<TValue\>, MessageData\<TValue\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<MessageData\<TValue\>, TValue\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **MessageDataPropertyConverter()**

```csharp
public MessageDataPropertyConverter()
```

## Methods

### **Convert\<T\>(InitializeContext\<T\>, MessageData\<TValue\>)**

```csharp
public Task<MessageData<TValue>> Convert<T>(InitializeContext<T> context, MessageData<TValue> input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [MessageData\<TValue\>](../../masstransit-abstractions/masstransit/messagedata-1)<br/>

#### Returns

[Task\<MessageData\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Convert\<T1\>(InitializeContext\<T1\>, TValue)**

```csharp
public Task<MessageData<TValue>> Convert<T1>(InitializeContext<T1> context, TValue input)
```

#### Type Parameters

`T1`<br/>

#### Parameters

`context` [InitializeContext\<T1\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` TValue<br/>

#### Returns

[Task\<MessageData\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
