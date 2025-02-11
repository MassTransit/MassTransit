---

title: SqlMessageNameFormatter

---

# SqlMessageNameFormatter

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class SqlMessageNameFormatter : IMessageNameFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlMessageNameFormatter](../masstransit-sqltransport-topology/sqlmessagenameformatter)<br/>
Implements [IMessageNameFormatter](../../masstransit-abstractions/masstransit-transports/imessagenameformatter)

## Constructors

### **SqlMessageNameFormatter(String)**

```csharp
public SqlMessageNameFormatter(string namespaceSeparator)
```

#### Parameters

`namespaceSeparator` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **GetMessageName(Type)**

```csharp
public string GetMessageName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
