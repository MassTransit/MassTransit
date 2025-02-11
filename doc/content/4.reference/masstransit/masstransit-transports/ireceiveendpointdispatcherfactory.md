---

title: IReceiveEndpointDispatcherFactory

---

# IReceiveEndpointDispatcherFactory

Namespace: MassTransit.Transports

```csharp
public interface IReceiveEndpointDispatcherFactory : IAsyncDisposable
```

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Methods

### **CreateReceiver(String)**

Creates a single receiver with all configured consumers, sagas, etc.
 Note that if any other receivers are created for specific consumers or sagas, those consumers and sagas will
 not be included in this receiver as they've already been configured.

```csharp
IReceiveEndpointDispatcher CreateReceiver(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher)<br/>

### **CreateConsumerReceiver\<T\>(String)**

```csharp
IReceiveEndpointDispatcher CreateConsumerReceiver<T>(string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher)<br/>

### **CreateSagaReceiver\<T\>(String)**

```csharp
IReceiveEndpointDispatcher CreateSagaReceiver<T>(string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher)<br/>

### **CreateExecuteActivityReceiver\<T\>(String)**

```csharp
IReceiveEndpointDispatcher CreateExecuteActivityReceiver<T>(string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IReceiveEndpointDispatcher](../masstransit-transports/ireceiveendpointdispatcher)<br/>
