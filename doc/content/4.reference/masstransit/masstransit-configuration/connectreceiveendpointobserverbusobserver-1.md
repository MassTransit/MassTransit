---

title: ConnectReceiveEndpointObserverBusObserver<T>

---

# ConnectReceiveEndpointObserverBusObserver\<T\>

Namespace: MassTransit.Configuration

```csharp
public class ConnectReceiveEndpointObserverBusObserver<T> : IBusObserver
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConnectReceiveEndpointObserverBusObserver\<T\>](../masstransit-configuration/connectreceiveendpointobserverbusobserver-1)<br/>
Implements [IBusObserver](../../masstransit-abstractions/masstransit/ibusobserver)

## Constructors

### **ConnectReceiveEndpointObserverBusObserver(IServiceProvider)**

```csharp
public ConnectReceiveEndpointObserverBusObserver(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

## Methods

### **PostCreate(IBus)**

```csharp
public void PostCreate(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

### **CreateFaulted(Exception)**

```csharp
public void CreateFaulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **PreStart(IBus)**

```csharp
public Task PreStart(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostStart(IBus, Task\<BusReady\>)**

```csharp
public Task PostStart(IBus bus, Task<BusReady> busReady)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

`busReady` [Task\<BusReady\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StartFaulted(IBus, Exception)**

```csharp
public Task StartFaulted(IBus bus, Exception exception)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreStop(IBus)**

```csharp
public Task PreStop(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostStop(IBus)**

```csharp
public Task PostStop(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StopFaulted(IBus, Exception)**

```csharp
public Task StopFaulted(IBus bus, Exception exception)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
