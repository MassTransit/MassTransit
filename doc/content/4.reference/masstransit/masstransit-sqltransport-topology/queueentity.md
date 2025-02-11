---

title: QueueEntity

---

# QueueEntity

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class QueueEntity : Queue, QueueHandle, EntityHandle
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [QueueEntity](../masstransit-sqltransport-topology/queueentity)<br/>
Implements [Queue](../masstransit-sqltransport-topology/queue), [QueueHandle](../masstransit-sqltransport-topology/queuehandle), [EntityHandle](../masstransit-topology/entityhandle)

## Properties

### **NameComparer**

```csharp
public static IEqualityComparer<QueueEntity> NameComparer { get; }
```

#### Property Value

[IEqualityComparer\<QueueEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

### **QueueComparer**

```csharp
public static IEqualityComparer<QueueEntity> QueueComparer { get; }
```

#### Property Value

[IEqualityComparer\<QueueEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

### **QueueName**

```csharp
public string QueueName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AutoDeleteOnIdle**

```csharp
public Nullable<TimeSpan> AutoDeleteOnIdle { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MaxDeliveryCount**

```csharp
public Nullable<int> MaxDeliveryCount { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Id**

```csharp
public long Id { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Queue**

```csharp
public Queue Queue { get; }
```

#### Property Value

[Queue](../masstransit-sqltransport-topology/queue)<br/>

## Constructors

### **QueueEntity(Int64, String, Nullable\<TimeSpan\>, Nullable\<Int32\>)**

```csharp
public QueueEntity(long id, string name, Nullable<TimeSpan> autoDeleteOnIdle, Nullable<int> maxDeliveryCount)
```

#### Parameters

`id` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`autoDeleteOnIdle` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`maxDeliveryCount` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
