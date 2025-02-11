---

title: RoutingSlipExtensions

---

# RoutingSlipExtensions

Namespace: MassTransit

```csharp
public static class RoutingSlipExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipExtensions](../masstransit/routingslipextensions)

## Methods

### **RanToCompletion(RoutingSlip)**

Returns true if there are no remaining activities to be executed

```csharp
public static bool RanToCompletion(RoutingSlip routingSlip)
```

#### Parameters

`routingSlip` [RoutingSlip](../masstransit-courier-contracts/routingslip)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetNextExecuteAddress(RoutingSlip)**

```csharp
public static Uri GetNextExecuteAddress(RoutingSlip routingSlip)
```

#### Parameters

`routingSlip` [RoutingSlip](../masstransit-courier-contracts/routingslip)<br/>

#### Returns

Uri<br/>

### **GetNextCompensateAddress(RoutingSlip)**

```csharp
public static Uri GetNextCompensateAddress(RoutingSlip routingSlip)
```

#### Parameters

`routingSlip` [RoutingSlip](../masstransit-courier-contracts/routingslip)<br/>

#### Returns

Uri<br/>

### **Execute\<T\>(T, RoutingSlip)**

```csharp
public static Task Execute<T>(T source, RoutingSlip routingSlip)
```

#### Type Parameters

`T`<br/>

#### Parameters

`source` T<br/>

`routingSlip` [RoutingSlip](../masstransit-courier-contracts/routingslip)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Execute\<T\>(T, RoutingSlip, CancellationToken)**

```csharp
public static Task Execute<T>(T source, RoutingSlip routingSlip, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`source` T<br/>

`routingSlip` [RoutingSlip](../masstransit-courier-contracts/routingslip)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
