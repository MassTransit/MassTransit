---

title: SendConsumeContextExtensions

---

# SendConsumeContextExtensions

Namespace: MassTransit

```csharp
public static class SendConsumeContextExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendConsumeContextExtensions](../masstransit/sendconsumecontextextensions)

## Methods

### **Send\<T\>(ConsumeContext, Uri, T)**

Send a message

```csharp
public static Task Send<T>(ConsumeContext context, Uri destinationAddress, T message)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` T<br/>
The message

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ConsumeContext, Uri, T, IPipe\<SendContext\<T\>\>)**

Send a message

```csharp
public static Task Send<T>(ConsumeContext context, Uri destinationAddress, T message, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ConsumeContext, Uri, T, IPipe\<SendContext\>)**

Send a message

```csharp
public static Task Send<T>(ConsumeContext context, Uri destinationAddress, T message, IPipe<SendContext> pipe)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ConsumeContext, Uri, Object)**

Send a message

```csharp
public static Task Send(ConsumeContext context, Uri destinationAddress, object message)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ConsumeContext, Uri, Object, Type)**

Send a message

```csharp
public static Task Send(ConsumeContext context, Uri destinationAddress, object message, Type messageType)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ConsumeContext, Uri, Object, Type, IPipe\<SendContext\>)**

Send a message

```csharp
public static Task Send(ConsumeContext context, Uri destinationAddress, object message, Type messageType, IPipe<SendContext> pipe)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ConsumeContext, Uri, Object, IPipe\<SendContext\>)**

Send a message

```csharp
public static Task Send(ConsumeContext context, Uri destinationAddress, object message, IPipe<SendContext> pipe)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ConsumeContext, Uri, Object)**

Send a message

```csharp
public static Task Send<T>(ConsumeContext context, Uri destinationAddress, object values)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ConsumeContext, Uri, Object, IPipe\<SendContext\<T\>\>)**

Send a message

```csharp
public static Task Send<T>(ConsumeContext context, Uri destinationAddress, object values, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ConsumeContext, Uri, Object, IPipe\<SendContext\>)**

Send a message

```csharp
public static Task Send<T>(ConsumeContext context, Uri destinationAddress, object values, IPipe<SendContext> pipe)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

`destinationAddress` Uri<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker
