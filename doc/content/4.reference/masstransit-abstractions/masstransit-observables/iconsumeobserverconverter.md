---

title: IConsumeObserverConverter

---

# IConsumeObserverConverter

Namespace: MassTransit.Observables

Calls the generic version of the IPublishEndpoint.Send method with the object's type

```csharp
public interface IConsumeObserverConverter
```

## Methods

### **PreConsume(IConsumeObserver, Object)**

```csharp
Task PreConsume(IConsumeObserver observer, object context)
```

#### Parameters

`observer` [IConsumeObserver](../masstransit/iconsumeobserver)<br/>

`context` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume(IConsumeObserver, Object)**

```csharp
Task PostConsume(IConsumeObserver observer, object context)
```

#### Parameters

`observer` [IConsumeObserver](../masstransit/iconsumeobserver)<br/>

`context` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConsumeFault(IConsumeObserver, Object, Exception)**

```csharp
Task ConsumeFault(IConsumeObserver observer, object context, Exception exception)
```

#### Parameters

`observer` [IConsumeObserver](../masstransit/iconsumeobserver)<br/>

`context` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
