---

title: PerformanceCounterExtensions

---

# PerformanceCounterExtensions

Namespace: MassTransit

```csharp
public static class PerformanceCounterExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PerformanceCounterExtensions](../masstransit/performancecounterextensions)

## Methods

### **EnableStatsdPerformanceCounters(IBusFactoryConfigurator, Action\<StatsDConfiguration\>)**

```csharp
public static void EnableStatsdPerformanceCounters(IBusFactoryConfigurator configurator, Action<StatsDConfiguration> action)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`action` [Action\<StatsDConfiguration\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
