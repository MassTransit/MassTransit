---

title: InMemoryInboxMessageKey

---

# InMemoryInboxMessageKey

Namespace: MassTransit.Middleware.Outbox

```csharp
public struct InMemoryInboxMessageKey
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [InMemoryInboxMessageKey](../masstransit-middleware-outbox/inmemoryinboxmessagekey)

## Fields

### **MessageId**

```csharp
public Guid MessageId;
```

### **ConsumerId**

```csharp
public Guid ConsumerId;
```

## Properties

### **Comparer**

```csharp
public static IEqualityComparer<InMemoryInboxMessageKey> Comparer { get; }
```

#### Property Value

[IEqualityComparer\<InMemoryInboxMessageKey\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

## Constructors

### **InMemoryInboxMessageKey(Guid, Guid)**

```csharp
public InMemoryInboxMessageKey(Guid messageId, Guid consumerId)
```

#### Parameters

`messageId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`consumerId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
