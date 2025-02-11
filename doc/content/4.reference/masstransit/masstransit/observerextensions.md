---

title: ObserverExtensions

---

# ObserverExtensions

Namespace: MassTransit

```csharp
public static class ObserverExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ObserverExtensions](../masstransit/observerextensions)

## Methods

### **Observer\<T\>(IReceiveEndpointConfigurator, IObserver\<ConsumeContext\<T\>\>, Action\<IObserverConfigurator\<T\>\>)**

Subscribes an observer instance to the bus

```csharp
public static void Observer<T>(IReceiveEndpointConfigurator configurator, IObserver<ConsumeContext<T>> observer, Action<IObserverConfigurator<T>> configureCallback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
Service Bus Service Configurator
 - the item that is passed as a parameter to
 the action that is calling the configurator.

`observer` [IObserver\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>
The observer to connect to the endpoint

`configureCallback` [Action\<IObserverConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConnectObserver\<T\>(IBus, IObserver\<ConsumeContext\<T\>\>)**

Adds a message observer to the service bus for handling a specific type of message

```csharp
public static ConnectHandle ConnectObserver<T>(IBus bus, IObserver<ConsumeContext<T>> observer)
```

#### Type Parameters

`T`<br/>
The message type to handle, often inferred from the callback specified

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

`observer` [IObserver\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>
The callback to invoke when messages of the specified type arrive on the service bus

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectRequestObserver\<T\>(IBus, Guid, IObserver\<ConsumeContext\<T\>\>)**

Subscribe a request observer to the bus's endpoint

```csharp
public static ConnectHandle ConnectRequestObserver<T>(IBus bus, Guid requestId, IObserver<ConsumeContext<T>> observer)
```

#### Type Parameters

`T`<br/>

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`observer` [IObserver\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.iobserver-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
