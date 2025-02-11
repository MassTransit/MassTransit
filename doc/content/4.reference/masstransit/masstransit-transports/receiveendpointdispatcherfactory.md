---

title: ReceiveEndpointDispatcherFactory

---

# ReceiveEndpointDispatcherFactory

Namespace: MassTransit.Transports

```csharp
public class ReceiveEndpointDispatcherFactory : IReceiveEndpointDispatcherFactory, IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointDispatcherFactory](../masstransit-transports/receiveendpointdispatcherfactory)<br/>
Implements [IReceiveEndpointDispatcherFactory](../masstransit-transports/ireceiveendpointdispatcherfactory), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Constructors

### **ReceiveEndpointDispatcherFactory(IBusRegistrationContext, IBusInstance)**

```csharp
public ReceiveEndpointDispatcherFactory(IBusRegistrationContext registration, IBusInstance busInstance)
```

#### Parameters

`registration` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>

`busInstance` [IBusInstance](../masstransit-transports/ibusinstance)<br/>

## Methods

### **CreateReceiver(String)**

```csharp
public IReceiveEndpointDispatcher CreateReceiver(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher)<br/>

### **CreateConsumerReceiver\<T\>(String)**

```csharp
public IReceiveEndpointDispatcher CreateConsumerReceiver<T>(string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher)<br/>

### **CreateSagaReceiver\<T\>(String)**

```csharp
public IReceiveEndpointDispatcher CreateSagaReceiver<T>(string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher)<br/>

### **CreateExecuteActivityReceiver\<T\>(String)**

```csharp
public IReceiveEndpointDispatcher CreateExecuteActivityReceiver<T>(string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher)<br/>

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
