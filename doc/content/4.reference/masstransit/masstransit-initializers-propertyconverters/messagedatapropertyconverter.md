---

title: MessageDataPropertyConverter

---

# MessageDataPropertyConverter

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class MessageDataPropertyConverter : IPropertyConverter<MessageData<Byte[]>, MessageData<Byte[]>>, IPropertyConverter<MessageData<Byte[]>, MessageData<String>>, IPropertyConverter<MessageData<String>, MessageData<String>>, IPropertyConverter<MessageData<Stream>, MessageData<Stream>>, IPropertyConverter<MessageData<String>, String>, IPropertyConverter<MessageData<Byte[]>, String>, IPropertyConverter<MessageData<Byte[]>, Byte[]>, IPropertyConverter<MessageData<Stream>, Stream>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataPropertyConverter](../masstransit-initializers-propertyconverters/messagedatapropertyconverter)<br/>
Implements [IPropertyConverter\<MessageData\<Byte[]\>, MessageData\<Byte[]\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<MessageData\<Byte[]\>, MessageData\<String\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<MessageData\<String\>, MessageData\<String\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<MessageData\<Stream\>, MessageData\<Stream\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<MessageData\<String\>, String\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<MessageData\<Byte[]\>, String\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<MessageData\<Byte[]\>, Byte[]\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<MessageData\<Stream\>, Stream\>](../masstransit-initializers/ipropertyconverter-2)

## Fields

### **Instance**

```csharp
public static MessageDataPropertyConverter Instance;
```

## Methods

### **Convert\<T\>(InitializeContext\<T\>, Byte[])**

```csharp
public Task<MessageData<Byte[]>> Convert<T>(InitializeContext<T> context, Byte[] input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[Task\<MessageData\<Byte[]\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Convert\<T\>(InitializeContext\<T\>, MessageData\<Byte[]\>)**

```csharp
public Task<MessageData<Byte[]>> Convert<T>(InitializeContext<T> context, MessageData<Byte[]> input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [MessageData\<Byte[]\>](../../masstransit-abstractions/masstransit/messagedata-1)<br/>

#### Returns

[Task\<MessageData\<Byte[]\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Convert\<T\>(InitializeContext\<T\>, MessageData\<Stream\>)**

```csharp
public Task<MessageData<Stream>> Convert<T>(InitializeContext<T> context, MessageData<Stream> input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [MessageData\<Stream\>](../../masstransit-abstractions/masstransit/messagedata-1)<br/>

#### Returns

[Task\<MessageData\<Stream\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Convert\<T\>(InitializeContext\<T\>, Stream)**

```csharp
public Task<MessageData<Stream>> Convert<T>(InitializeContext<T> context, Stream input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

#### Returns

[Task\<MessageData\<Stream\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
