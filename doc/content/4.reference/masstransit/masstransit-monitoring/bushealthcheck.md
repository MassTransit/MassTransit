---

title: BusHealthCheck

---

# BusHealthCheck

Namespace: MassTransit.Monitoring

```csharp
public class BusHealthCheck : IHealthCheck
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusHealthCheck](../masstransit-monitoring/bushealthcheck)<br/>
Implements IHealthCheck

## Constructors

### **BusHealthCheck(IBusInstance)**

```csharp
public BusHealthCheck(IBusInstance busInstance)
```

#### Parameters

`busInstance` [IBusInstance](../masstransit-transports/ibusinstance)<br/>

## Methods

### **CheckHealthAsync(HealthCheckContext, CancellationToken)**

```csharp
public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
```

#### Parameters

`context` HealthCheckContext<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<HealthCheckResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
