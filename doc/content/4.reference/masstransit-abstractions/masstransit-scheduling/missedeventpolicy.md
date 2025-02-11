---

title: MissedEventPolicy

---

# MissedEventPolicy

Namespace: MassTransit.Scheduling

If the scheduler is offline and comes back online, the policy determines how
 a missed scheduled message is handled.

```csharp
public enum MissedEventPolicy
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [MissedEventPolicy](../masstransit-scheduling/missedeventpolicy)<br/>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Default | 0 | use the default handling of the scheduler |
| Skip | 1 | Skip the event, waiting for the next scheduled interval |
| Send | 2 | Send the message immediately and then continue the schedule as planned |
