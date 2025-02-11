---

title: SqlQueueConfigurator

---

# SqlQueueConfigurator

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlQueueConfigurator : ISqlQueueConfigurator, Queue
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlQueueConfigurator](../masstransit-sqltransport-configuration/sqlqueueconfigurator)<br/>
Implements [ISqlQueueConfigurator](../masstransit/isqlqueueconfigurator), [Queue](../masstransit-sqltransport-topology/queue)

## Properties

### **AutoDeleteOnIdle**

```csharp
public Nullable<TimeSpan> AutoDeleteOnIdle { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MaxDeliveryCount**

```csharp
public Nullable<int> MaxDeliveryCount { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **QueueName**

```csharp
public string QueueName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **GetEndpointAddress(Uri)**

```csharp
protected SqlEndpointAddress GetEndpointAddress(Uri hostAddress)
```

#### Parameters

`hostAddress` Uri<br/>

#### Returns

[SqlEndpointAddress](../masstransit/sqlendpointaddress)<br/>
