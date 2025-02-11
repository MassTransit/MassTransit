---

title: PendingTaskCollection

---

# PendingTaskCollection

Namespace: MassTransit.Util

```csharp
public class PendingTaskCollection
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PendingTaskCollection](../masstransit-util/pendingtaskcollection)

## Constructors

### **PendingTaskCollection(Int32)**

```csharp
public PendingTaskCollection(int capacity)
```

#### Parameters

`capacity` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Add(IEnumerable\<Task\>)**

```csharp
public void Add(IEnumerable<Task> tasks)
```

#### Parameters

`tasks` [IEnumerable\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Add(Task)**

```csharp
public void Add(Task task)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed(CancellationToken)**

```csharp
public Task Completed(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
