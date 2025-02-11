---

title: PublishObservable

---

# PublishObservable

Namespace: MassTransit.Observables

```csharp
public class PublishObservable : Connectable<IPublishObserver>, IPublishObserver, ISendObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IPublishObserver\>](../masstransit-util/connectable-1) → [PublishObservable](../masstransit-observables/publishobservable)<br/>
Implements [IPublishObserver](../masstransit/ipublishobserver), [ISendObserver](../masstransit/isendobserver)

## Properties

### **Connected**

```csharp
public IPublishObserver[] Connected { get; }
```

#### Property Value

[IPublishObserver[]](../masstransit/ipublishobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **PublishObservable()**

```csharp
public PublishObservable()
```

## Methods

### **PrePublish\<T\>(PublishContext\<T\>)**

```csharp
public Task PrePublish<T>(PublishContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../masstransit/publishcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostPublish\<T\>(PublishContext\<T\>)**

```csharp
public Task PostPublish<T>(PublishContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../masstransit/publishcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishFault\<T\>(PublishContext\<T\>, Exception)**

```csharp
public Task PublishFault<T>(PublishContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../masstransit/publishcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreSend\<T\>(SendContext\<T\>)**

```csharp
public Task PreSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostSend\<T\>(SendContext\<T\>)**

```csharp
public Task PostSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendFault\<T\>(SendContext\<T\>, Exception)**

```csharp
public Task SendFault<T>(SendContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../masstransit/sendcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
