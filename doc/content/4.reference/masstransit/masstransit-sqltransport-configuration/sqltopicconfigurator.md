---

title: SqlTopicConfigurator

---

# SqlTopicConfigurator

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlTopicConfigurator : ISqlTopicConfigurator, Topic
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlTopicConfigurator](../masstransit-sqltransport-configuration/sqltopicconfigurator)<br/>
Implements [ISqlTopicConfigurator](../masstransit/isqltopicconfigurator), [Topic](../masstransit-sqltransport-topology/topic)

## Properties

### **TopicName**

```csharp
public string TopicName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **SqlTopicConfigurator(String)**

```csharp
public SqlTopicConfigurator(string topicName)
```

#### Parameters

`topicName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **GetEndpointAddress(Uri)**

```csharp
public SqlEndpointAddress GetEndpointAddress(Uri hostAddress)
```

#### Parameters

`hostAddress` Uri<br/>

#### Returns

[SqlEndpointAddress](../masstransit/sqlendpointaddress)<br/>
