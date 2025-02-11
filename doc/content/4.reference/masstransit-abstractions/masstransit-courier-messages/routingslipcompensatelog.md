---

title: RoutingSlipCompensateLog

---

# RoutingSlipCompensateLog

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipCompensateLog : CompensateLog
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipCompensateLog](../masstransit-courier-messages/routingslipcompensatelog)<br/>
Implements [CompensateLog](../masstransit-courier-contracts/compensatelog)

## Properties

### **ExecutionId**

```csharp
public Guid ExecutionId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Address**

```csharp
public Uri Address { get; set; }
```

#### Property Value

Uri<br/>

### **Data**

```csharp
public IDictionary<string, object> Data { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

## Constructors

### **RoutingSlipCompensateLog()**

```csharp
public RoutingSlipCompensateLog()
```

### **RoutingSlipCompensateLog(Guid, Uri, IDictionary\<String, Object\>)**

```csharp
public RoutingSlipCompensateLog(Guid executionId, Uri address, IDictionary<string, object> data)
```

#### Parameters

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`address` Uri<br/>

`data` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **RoutingSlipCompensateLog(CompensateLog)**

```csharp
public RoutingSlipCompensateLog(CompensateLog compensateLog)
```

#### Parameters

`compensateLog` [CompensateLog](../masstransit-courier-contracts/compensatelog)<br/>
