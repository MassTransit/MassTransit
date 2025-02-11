---

title: SagaConsumeContextMode

---

# SagaConsumeContextMode

Namespace: MassTransit.Saga

```csharp
public enum SagaConsumeContextMode
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [SagaConsumeContextMode](../masstransit-saga/sagaconsumecontextmode)<br/>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Load | 0 | Existing saga loaded from storage |
| Add | 1 | New saga created |
| Insert | 2 | New saga inserted prior to event |
