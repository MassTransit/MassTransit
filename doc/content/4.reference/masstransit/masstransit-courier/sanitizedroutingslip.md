---

title: SanitizedRoutingSlip

---

# SanitizedRoutingSlip

Namespace: MassTransit.Courier

A sanitized routing slip is one that has been read from and ensured to be safe for use, cleaning up any
 missing or null properties, as well as making it safe to avoid excessive validity checks across the solution

```csharp
public class SanitizedRoutingSlip : RoutingSlip
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SanitizedRoutingSlip](../masstransit-courier/sanitizedroutingslip)<br/>
Implements [RoutingSlip](../../masstransit-abstractions/masstransit-courier-contracts/routingslip)

## Properties

### **TrackingNumber**

```csharp
public Guid TrackingNumber { get; private set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **CreateTimestamp**

```csharp
public DateTime CreateTimestamp { get; private set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Itinerary**

```csharp
public IList<Activity> Itinerary { get; private set; }
```

#### Property Value

[IList\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **ActivityLogs**

```csharp
public IList<ActivityLog> ActivityLogs { get; private set; }
```

#### Property Value

[IList\<ActivityLog\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **CompensateLogs**

```csharp
public IList<CompensateLog> CompensateLogs { get; private set; }
```

#### Property Value

[IList\<CompensateLog\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **Variables**

```csharp
public IDictionary<string, object> Variables { get; private set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **ActivityExceptions**

```csharp
public IList<ActivityException> ActivityExceptions { get; private set; }
```

#### Property Value

[IList\<ActivityException\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **Subscriptions**

```csharp
public IList<Subscription> Subscriptions { get; private set; }
```

#### Property Value

[IList\<Subscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

## Constructors

### **SanitizedRoutingSlip(ConsumeContext\<RoutingSlip\>)**

```csharp
public SanitizedRoutingSlip(ConsumeContext<RoutingSlip> context)
```

#### Parameters

`context` [ConsumeContext\<RoutingSlip\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

## Methods

### **GetActivityArguments\<T\>()**

```csharp
public T GetActivityArguments<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

T<br/>

### **GetCompensateLogData\<T\>()**

```csharp
public T GetCompensateLogData<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

T<br/>
