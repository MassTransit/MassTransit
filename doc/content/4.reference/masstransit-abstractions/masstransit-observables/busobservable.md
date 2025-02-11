---

title: BusObservable

---

# BusObservable

Namespace: MassTransit.Observables

```csharp
public class BusObservable : Connectable<IBusObserver>, IBusObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IBusObserver\>](../masstransit-util/connectable-1) → [BusObservable](../masstransit-observables/busobservable)<br/>
Implements [IBusObserver](../masstransit/ibusobserver)

## Properties

### **Connected**

```csharp
public IBusObserver[] Connected { get; }
```

#### Property Value

[IBusObserver[]](../masstransit/ibusobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **BusObservable()**

```csharp
public BusObservable()
```

## Methods

### **PostCreate(IBus)**

```csharp
public void PostCreate(IBus bus)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

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

`bus` [IBus](../masstransit/ibus)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostStart(IBus, Task\<BusReady\>)**

```csharp
public Task PostStart(IBus bus, Task<BusReady> busReady)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

`busReady` [Task\<BusReady\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StartFaulted(IBus, Exception)**

```csharp
public Task StartFaulted(IBus bus, Exception exception)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreStop(IBus)**

```csharp
public Task PreStop(IBus bus)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostStop(IBus)**

```csharp
public Task PostStop(IBus bus)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StopFaulted(IBus, Exception)**

```csharp
public Task StopFaulted(IBus bus, Exception exception)
```

#### Parameters

`bus` [IBus](../masstransit/ibus)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
