---

title: MessageDataExtensions

---

# MessageDataExtensions

Namespace: MassTransit

```csharp
public static class MessageDataExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataExtensions](../masstransit/messagedataextensions)

## Methods

### **PutString(IMessageDataRepository, String, CancellationToken)**

```csharp
public static Task<MessageData<string>> PutString(IMessageDataRepository repository, string value, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<MessageData\<String\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **PutString(IMessageDataRepository, String, Nullable\<TimeSpan\>, CancellationToken)**

```csharp
public static Task<MessageData<string>> PutString(IMessageDataRepository repository, string value, Nullable<TimeSpan> timeToLive, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`timeToLive` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<MessageData\<String\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **PutBytes(IMessageDataRepository, Byte[], CancellationToken)**

```csharp
public static Task<MessageData<Byte[]>> PutBytes(IMessageDataRepository repository, Byte[] bytes, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`bytes` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<MessageData\<Byte[]\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **PutBytes(IMessageDataRepository, Byte[], Nullable\<TimeSpan\>, CancellationToken)**

```csharp
public static Task<MessageData<Byte[]>> PutBytes(IMessageDataRepository repository, Byte[] bytes, Nullable<TimeSpan> timeToLive, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`bytes` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`timeToLive` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<MessageData\<Byte[]\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **PutObject(IMessageDataRepository, Object, Type, CancellationToken)**

```csharp
public static Task<IMessageData> PutObject(IMessageDataRepository repository, object value, Type objectType, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<IMessageData\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **PutObject(IMessageDataRepository, Object, Type, Nullable\<TimeSpan\>, CancellationToken)**

```csharp
public static Task<IMessageData> PutObject(IMessageDataRepository repository, object value, Type objectType, Nullable<TimeSpan> timeToLive, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`timeToLive` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<IMessageData\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **PutStream(IMessageDataRepository, Stream, CancellationToken)**

```csharp
public static Task<MessageData<Stream>> PutStream(IMessageDataRepository repository, Stream stream, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<MessageData\<Stream\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **PutStream(IMessageDataRepository, Stream, Nullable\<TimeSpan\>, CancellationToken)**

```csharp
public static Task<MessageData<Stream>> PutStream(IMessageDataRepository repository, Stream stream, Nullable<TimeSpan> timeToLive, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`timeToLive` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<MessageData\<Stream\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetString(IMessageDataRepository, Uri, CancellationToken)**

```csharp
public static Task<MessageData<string>> GetString(IMessageDataRepository repository, Uri address, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`address` Uri<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<MessageData\<String\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetBytes(IMessageDataRepository, Uri, CancellationToken)**

```csharp
public static Task<MessageData<Byte[]>> GetBytes(IMessageDataRepository repository, Uri address, CancellationToken cancellationToken)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

`address` Uri<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<MessageData\<Byte[]\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
