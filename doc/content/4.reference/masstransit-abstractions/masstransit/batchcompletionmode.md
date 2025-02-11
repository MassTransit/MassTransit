---

title: BatchCompletionMode

---

# BatchCompletionMode

Namespace: MassTransit

The reason this batch was made ready for consumption

```csharp
public enum BatchCompletionMode
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [BatchCompletionMode](../masstransit/batchcompletionmode)<br/>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Time | 0 | The time limit for receiving messages in the batch was reached |
| Size | 1 | The maximum number of messages in the batch was reached |
| Forced | 2 | A batch was forced, likely due to a previously faulted message being retried |
