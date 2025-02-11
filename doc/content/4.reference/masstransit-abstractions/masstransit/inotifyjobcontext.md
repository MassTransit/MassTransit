---

title: INotifyJobContext

---

# INotifyJobContext

Namespace: MassTransit

```csharp
public interface INotifyJobContext
```

## Methods

### **NotifyCanceled()**

```csharp
Task NotifyCanceled()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyStarted()**

```csharp
Task NotifyStarted()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyCompleted()**

```csharp
Task NotifyCompleted()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted(Exception, Nullable\<TimeSpan\>)**

```csharp
Task NotifyFaulted(Exception exception, Nullable<TimeSpan> delay)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`delay` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyJobProgress(SetJobProgress)**

```csharp
Task NotifyJobProgress(SetJobProgress progress)
```

#### Parameters

`progress` [SetJobProgress](../masstransit-contracts-jobservice/setjobprogress)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
