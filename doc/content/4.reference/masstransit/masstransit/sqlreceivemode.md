---

title: SqlReceiveMode

---

# SqlReceiveMode

Namespace: MassTransit

```csharp
public enum SqlReceiveMode
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [SqlReceiveMode](../masstransit/sqlreceivemode)<br/>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Normal | 0 | Messages are delivered normally in priority, enqueue_time order |
| Partitioned | 1 | Messages are delivered in priority enqueue_time order, but only one message per PartitionKey at a time |
| PartitionedConcurrent | 2 | Messages are delivered in priority enqueue_time order, with additional messages fetched from the server for the same PartitionKey |
| PartitionedOrdered | 3 | Messages are delivered in first-in first-out order, but only one message per PartitionKey at a time |
| PartitionedOrderedConcurrent | 4 | Messages are delivered in first-in first-out order, with additional messages fetched from the server for the same PartitionKey |
