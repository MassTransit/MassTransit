---

title: RoutingSlipEvents

---

# RoutingSlipEvents

Namespace: MassTransit.Courier.Contracts

```csharp
public enum RoutingSlipEvents
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [RoutingSlipEvents](../masstransit-courier-contracts/routingslipevents)<br/>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| All | 0 | By default, all routing slip events are included for a subscription |
| Completed | 1 | Send the RoutingSlipCompleted event |
| Faulted | 2 | Send the RoutingSlipFaulted event |
| CompensationFailed | 4 | Send the RoutingSlipCompensationFaulted event |
| Terminated | 8 | Send the routing slip terminated event |
| Revised | 16 | Send the routing slip revised event |
| ActivityCompleted | 256 | Send the RoutingSlipActivityCompleted event |
| ActivityFaulted | 512 | Send the RoutingSlipActivityFaulted event |
| ActivityCompensated | 1024 | Send the RoutingSlipActivityCompensated event |
| ActivityCompensationFailed | 2048 | Send the RoutingSlipCompensationFailed event |
| EventMask | 65535 | Used to mask the events so that upper-level flags don't conflict |
| Supplemental | 65536 | If specified, the event subscription is supplemental and should not prevent the publishing of existing routing slip events. By default, any subscription suppresses publishing of events. |
