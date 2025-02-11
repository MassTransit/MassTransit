---

title: RedeliveryOptions

---

# RedeliveryOptions

Namespace: MassTransit

Customize the redelivery experience

```csharp
public enum RedeliveryOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [RedeliveryOptions](../masstransit/redeliveryoptions)<br/>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| ReplaceMessageId | 1 | Generate a new MessageId for the redelivered message (typically to avoid broker deduplication logic) |
| UseMessageScheduler | 2 | If specified, use the message scheduler context instead of the redelivery context (only use when transport-level redelivery is not available) |
