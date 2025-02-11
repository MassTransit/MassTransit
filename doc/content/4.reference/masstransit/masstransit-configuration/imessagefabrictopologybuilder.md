---

title: IMessageFabricTopologyBuilder

---

# IMessageFabricTopologyBuilder

Namespace: MassTransit.Configuration

```csharp
public interface IMessageFabricTopologyBuilder
```

## Methods

### **ExchangeBind(String, String, String)**

```csharp
void ExchangeBind(string source, string destination, string routingKey)
```

#### Parameters

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **QueueBind(String, String)**

```csharp
void QueueBind(string source, string destination)
```

#### Parameters

`source` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExchangeDeclare(String, ExchangeType)**

```csharp
void ExchangeDeclare(string name, ExchangeType exchangeType)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

### **QueueDeclare(String)**

```csharp
void QueueDeclare(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
