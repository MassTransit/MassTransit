---

title: EndpointConventionExtensions

---

# EndpointConventionExtensions

Namespace: MassTransit

```csharp
public static class EndpointConventionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointConventionExtensions](../masstransit/endpointconventionextensions)

## Methods

### **Send\<T\>(ISendEndpointProvider, T, CancellationToken)**

Send a message

```csharp
public static Task Send<T>(ISendEndpointProvider provider, T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`message` T<br/>
The message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ISendEndpointProvider, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

Send a message

```csharp
public static Task Send<T>(ISendEndpointProvider provider, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ISendEndpointProvider, T, IPipe\<SendContext\>, CancellationToken)**

Send a message

```csharp
public static Task Send<T>(ISendEndpointProvider provider, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`message` T<br/>
The message

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ISendEndpointProvider, Object, CancellationToken)**

Send a message

```csharp
public static Task Send(ISendEndpointProvider provider, object message, CancellationToken cancellationToken)
```

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ISendEndpointProvider, Object, Type, CancellationToken)**

Send a message

```csharp
public static Task Send(ISendEndpointProvider provider, object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ISendEndpointProvider, Object, IPipe\<SendContext\>, CancellationToken)**

Send a message

```csharp
public static Task Send(ISendEndpointProvider provider, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send(ISendEndpointProvider, Object, Type, IPipe\<SendContext\>, CancellationToken)**

Send a message

```csharp
public static Task Send(ISendEndpointProvider provider, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The message

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ISendEndpointProvider, Object, CancellationToken)**

Send a message

```csharp
public static Task Send<T>(ISendEndpointProvider provider, object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ISendEndpointProvider, Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

Send a message

```csharp
public static Task Send<T>(ISendEndpointProvider provider, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker

### **Send\<T\>(ISendEndpointProvider, Object, IPipe\<SendContext\>, CancellationToken)**

Send a message

```csharp
public static Task Send<T>(ISendEndpointProvider provider, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
The task which is completed once the Send is acknowledged by the broker
