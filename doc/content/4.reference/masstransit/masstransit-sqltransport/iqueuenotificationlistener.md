---

title: IQueueNotificationListener

---

# IQueueNotificationListener

Namespace: MassTransit.SqlTransport

```csharp
public interface IQueueNotificationListener
```

## Methods

### **MessageReady(String)**

```csharp
Task MessageReady(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
