---

title: MessageLockContext

---

# MessageLockContext

Namespace: MassTransit.SqlTransport

```csharp
public interface MessageLockContext
```

## Methods

### **Complete()**

```csharp
Task Complete()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Abandon(Exception)**

```csharp
Task Abandon(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DeadLetter()**

```csharp
Task DeadLetter()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DeadLetter(Exception)**

```csharp
Task DeadLetter(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
