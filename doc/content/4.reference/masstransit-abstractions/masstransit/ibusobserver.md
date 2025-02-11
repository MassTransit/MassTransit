---

title: IBusObserver

---

# IBusObserver

Namespace: MassTransit

Used to observe events produced by the bus

```csharp
public interface IBusObserver
```

## Methods

### **PostCreate(IBus)**

Called after the bus has been created.

```csharp
void PostCreate(IBus bus)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

### **CreateFaulted(Exception)**

Called when the bus fails to be created

```csharp
void CreateFaulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **PreStart(IBus)**

Called when the bus is being started, before the actual Start commences.

```csharp
Task PreStart(IBus bus)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostStart(IBus, Task\<BusReady\>)**

Called once the bus has started and is running

```csharp
Task PostStart(IBus bus, Task<BusReady> busReady)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

`busReady` [Task\<BusReady\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
A task which is completed once the bus is ready and all receive endpoints are ready.

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StartFaulted(IBus, Exception)**

Called when the bus fails to start

```csharp
Task StartFaulted(IBus bus, Exception exception)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreStop(IBus)**

Called when the bus is being stopped, before the actual Stop commences.

```csharp
Task PreStop(IBus bus)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostStop(IBus)**

Called when the bus has been stopped.

```csharp
Task PostStop(IBus bus)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StopFaulted(IBus, Exception)**

Called when the bus failed to Stop.

```csharp
Task StopFaulted(IBus bus, Exception exception)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
