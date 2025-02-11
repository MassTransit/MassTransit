---

title: RoutingSlipEventContents

---

# RoutingSlipEventContents

Namespace: MassTransit.Courier.Contracts

Specifies the specific contents of routing slip events to be included for a subscription

```csharp
public enum RoutingSlipEventContents
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [RoutingSlipEventContents](../masstransit-courier-contracts/routingslipeventcontents)<br/>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| All | 0 | Include all event contents |
| None | 256 | Do not include any contents with the routing slip events |
| Variables | 1 | The routing slip variables after the activity was executed or compensated |
| Arguments | 2 | The arguments provided to the activity |
| Data | 4 | The data logged by an activity when completed or compensated |
| Itinerary | 8 | The itinerary that was added/removed from the routing slip when revised |
| SkipEncrypted | 256 | If specified, encrypted content is excluded from the event |
