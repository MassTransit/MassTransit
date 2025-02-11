---

title: RawSerializerOptions

---

# RawSerializerOptions

Namespace: MassTransit

```csharp
public enum RawSerializerOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [RawSerializerOptions](../masstransit/rawserializeroptions)<br/>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| AnyMessageType | 1 | Any message type is allowed, the supported message type array values are not checked |
| AddTransportHeaders | 2 | Add the transport headers on the outbound message |
| CopyHeaders | 4 | Copy message headers to outbound messages |
