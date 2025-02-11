---

title: SqlHeaderProvider

---

# SqlHeaderProvider

Namespace: MassTransit.SqlTransport

```csharp
public class SqlHeaderProvider : IHeaderProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlHeaderProvider](../masstransit-sqltransport/sqlheaderprovider)<br/>
Implements [IHeaderProvider](../../masstransit-abstractions/masstransit-transports/iheaderprovider)

## Constructors

### **SqlHeaderProvider(SqlTransportMessage)**

```csharp
public SqlHeaderProvider(SqlTransportMessage message)
```

#### Parameters

`message` [SqlTransportMessage](../masstransit-sqltransport/sqltransportmessage)<br/>

## Methods

### **GetAll()**

```csharp
public IEnumerable<KeyValuePair<string, object>> GetAll()
```

#### Returns

[IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **TryGetHeader(String, Object)**

```csharp
public bool TryGetHeader(string key, out object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
