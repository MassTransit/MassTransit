---

title: InMemoryEndpointAddress

---

# InMemoryEndpointAddress

Namespace: MassTransit

```csharp
public struct InMemoryEndpointAddress
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [InMemoryEndpointAddress](../masstransit/inmemoryendpointaddress)

## Fields

### **Scheme**

```csharp
public string Scheme;
```

### **Host**

```csharp
public string Host;
```

### **VirtualHost**

```csharp
public string VirtualHost;
```

### **Name**

```csharp
public string Name;
```

### **BindToQueue**

```csharp
public bool BindToQueue;
```

### **QueueName**

```csharp
public string QueueName;
```

### **ExchangeType**

```csharp
public ExchangeType ExchangeType;
```

## Constructors

### **InMemoryEndpointAddress(Uri, Uri)**

```csharp
public InMemoryEndpointAddress(Uri hostAddress, Uri address)
```

#### Parameters

`hostAddress` Uri<br/>

`address` Uri<br/>

### **InMemoryEndpointAddress(Uri, String, Boolean, String, ExchangeType)**

```csharp
public InMemoryEndpointAddress(Uri hostAddress, string exchangeName, bool bindToQueue, string queueName, ExchangeType exchangeType)
```

#### Parameters

`hostAddress` Uri<br/>

`exchangeName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`bindToQueue` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>
