---

title: EntityNameAttribute

---

# EntityNameAttribute

Namespace: MassTransit

Specify the EntityName used for this message contract
 if configured.

```csharp
public class EntityNameAttribute : Attribute
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://learn.microsoft.com/en-us/dotnet/api/system.attribute) → [EntityNameAttribute](../masstransit/entitynameattribute)

## Properties

### **EntityName**

```csharp
public string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **EntityNameAttribute(String)**



```csharp
public EntityNameAttribute(string entityName)
```

#### Parameters

`entityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The entity name to use for the message type
