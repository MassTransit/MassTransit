---

title: CompositeEventOptions

---

# CompositeEventOptions

Namespace: MassTransit

```csharp
public enum CompositeEventOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [CompositeEventOptions](../masstransit/compositeeventoptions)<br/>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| IncludeInitial | 1 | Include the composite event in the initial state |
| IncludeFinal | 2 | Include the composite event in the final state |
| RaiseOnce | 4 | Specifies that the composite event should only be raised once and ignore any subsequent events |
